using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Localization;
using WKClientsImporter.Models;
using WKClientsImporter.Models.Validators;

namespace WKClientsImporter.Views
{
    public partial class MainForm : Form
    {
        private BindingList<Cliente> _clientes;
        private readonly IStorageService _storageService;
        private readonly IDataImporter _importerService;
        private readonly ITemplateBuilder _templateBuilder;
        private readonly IStringLocalizer _localizer;
        private readonly ILogger _logger;

        // Flag para evitar reentradas al sincronizar el combo con el localizer
        private bool _suppressLanguageSelectionChange;

        public MainForm(IStorageService storageService, IDataImporter importerService,
            ITemplateBuilder templateBuilder, IStringLocalizer localizer, ILogger logger)
        {
            InitializeComponent();
            _storageService = storageService;
            _importerService = importerService;
            _templateBuilder = templateBuilder;
            _localizer = localizer;
            _logger = logger;
            ApplyLocalizer();
            LoadInitialData();
            InitializeGridValidations();
        }

        private void LoadInitialData()
        {
            var data = _storageService.Load() ?? new List<Cliente>();
            _clientes = new BindingList<Cliente>(data);
            dgvClientes.DataSource = _clientes;
        }

        #region Localization

        private void ApplyLocalizer()
        {
            // Subscribe to language change event
            _localizer.LanguageChanged += Localizer_LanguageChanged;

            // Subscribe to ComboBox change event
            cbLanguage.SelectedIndexChanged += CbLanguage_SelectedIndexChanged;

            PopulateLanguageCombo();
            ReloadLocalization();
        }

        private void PopulateLanguageCombo()
        {
            // Evitar que seleccionar programáticamente dispare el handler
            _suppressLanguageSelectionChange = true;

            var langs = _localizer.GetAvailableLanguages();
            cbLanguage.DataSource = langs;
            cbLanguage.DisplayMember = "Value";
            cbLanguage.ValueMember = "Key";

            var current = _localizer.CurrentLanguage ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (langs.Any(k => string.Equals(k.Key, current, StringComparison.OrdinalIgnoreCase)))
            {
                cbLanguage.SelectedValue = current;
            }
            else if (langs.Count > 0)
            {
                cbLanguage.SelectedIndex = 0;
            }

            _suppressLanguageSelectionChange = false;
        }

        private void CbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressLanguageSelectionChange) return;

            // SelectedValue viene del ValueMember ("Key") y debe ser el código de idioma
            var selected = cbLanguage.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(selected)) return;

            // Si ya es el idioma actual, no hacemos nada
            if (string.Equals(selected, _localizer.CurrentLanguage, StringComparison.OrdinalIgnoreCase)) return;

            try
            {
                // Evitar que la actualización del localizer vuelva a disparar el cambio de selección
                _suppressLanguageSelectionChange = true;
                _localizer.SetLanguage(selected);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error cambiando idioma.", ex);
            }
            finally
            {
                _suppressLanguageSelectionChange = false;
            }
        }

        private void Localizer_LanguageChanged(object sender, EventArgs e)
        {
            // Actualizar textos en UI y sincronizar selección del combo
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    SyncLanguageComboWithLocalizer();
                    ReloadLocalization();
                }));
            }
            else
            {
                SyncLanguageComboWithLocalizer();
                ReloadLocalization();
            }
        }

        private void SyncLanguageComboWithLocalizer()
        {
            try
            {
                _suppressLanguageSelectionChange = true;
                var current = _localizer.CurrentLanguage ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                // Si el DataSource no contiene el idioma actual, repoblar (por si han cambiado archivos en disco)
                var langs = cbLanguage.DataSource as List<KeyValuePair<string, string>>;
                if (langs == null || !langs.Any(k => string.Equals(k.Key, current, StringComparison.OrdinalIgnoreCase)))
                {
                    PopulateLanguageCombo();
                }
                else
                {
                    cbLanguage.SelectedValue = current;
                }
            }
            finally
            {
                _suppressLanguageSelectionChange = false;
            }
        }

        private void ReloadLocalization()
        {
            try
            {
                this.Text = _localizer.Get("MainFormTitle");
                btnImport.Text = _localizer.Get("ButtonImport");
                btnTemplate.Text = _localizer.Get("ButtonTemplate");
                // Reload any other texts
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error applying localization.", ex);
            }
        }

        #endregion

        #region Events

        private async void btnImport_Click(object sender, EventArgs e)
        {
            var filterExtensions = GetFileFilterExtensions();
            OpenFileDialog dialog = new OpenFileDialog { Filter = filterExtensions };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var progress = new Progress<int>(v =>
                {
                    pbImport.Value = Math.Min(Math.Max(v, 0), 100);
                    lblProgressPercent.Text = $"{pbImport.Value}%";
                });

                try
                {
                    var importedData = await _importerService.ImportAsync(dialog.FileName, progress);

                    // Contadores para resumen
                    int added = 0;
                    int updated = 0;
                    int ignored = 0;

                    for (int i = 0; i < importedData.Count; i++)
                    {
                        var item = importedData[i];

                        // Si falta DNI no se puede identificar: ignorar y loguear
                        if (item == null || string.IsNullOrWhiteSpace(item.DNI))
                        {
                            ignored++;
                            _logger?.LogWarning($"Registro importado sin DNI. Índice importado: {i + 1}. Nombre={item?.Nombre}");
                            continue;
                        }

                        // Buscar existente por DNI (clave única)
                        var existing = _clientes.FirstOrDefault(c => string.Equals(c.DNI, item.DNI, StringComparison.OrdinalIgnoreCase));
                        if (existing != null)
                        {
                            // Actualizar campos del existente
                            existing.Nombre = item.Nombre;
                            existing.Apellidos = item.Apellidos;
                            existing.FechaNacimiento = item.FechaNacimiento;
                            existing.Email = item.Email;
                            existing.Telefono = item.Telefono;

                            updated++;
                        }
                        else
                        {
                            // Nuevo registro
                            _clientes.Add(item);
                            added++;
                        }
                    }

                    // Forzar refresco de la cuadrícula por si no hay INotifyPropertyChanged en Cliente
                    dgvClientes.Refresh();

                    // Re-aplicar coloreado según validaciones
                    ApplyValidationHighlighting();

                    _logger?.LogInfo($"Importación finalizada. Total importados: {importedData.Count}. Añadidos: {added}. Actualizados: {updated}. Ignorados: {ignored}.");
                    MessageBox.Show($"Importación finalizada.\nTotal leídos: {importedData.Count}\nAñadidos: {added}\nActualizados: {updated}\nIgnorados: {ignored}",
                        _localizer.Get("InformationTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(_localizer.Get("ErrorImporting", dialog.FileName), ex);
                    MessageBox.Show(_localizer.Get("ErrorImporting", ex.Message), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnTemplate_Click(object sender, EventArgs e)
        {
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            if (fileExtensions == null || fileExtensions.Count == 0)
            {
                _logger?.LogInfo(_localizer.Get("NoSupportedFileFormats"));
                MessageBox.Show(_localizer.Get("NoSupportedFileFormats"), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var formatDialog = new TemplateFormatDialog(_localizer, fileExtensions))
            {
                if (formatDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var selectedExt = formatDialog.SelectedExtension;
                if (string.IsNullOrWhiteSpace(selectedExt))
                {
                    _logger?.LogInfo(_localizer.Get("NoFormatSelected"));
                    MessageBox.Show(_localizer.Get("NoFormatSelected"), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var extWithoutDot = selectedExt.TrimStart('.').ToUpperInvariant();
                var filter = $"{extWithoutDot} Files|*{selectedExt}";

                using (var dialog = new SaveFileDialog { Filter = filter, FileName = $"clients_template{selectedExt}" })
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    try
                    {
                        await _templateBuilder.BuildTemplateAsync(dialog.FileName, selectedExt);
                        _logger?.LogInfo($"{_localizer.Get("TemplateCreated")}. Filename: {dialog.FileName}");
                        MessageBox.Show(_localizer.Get("TemplateCreated"), _localizer.Get("TemplateCreatedTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(_localizer.Get("ErrorCreatingTemplate", dialog.FileName), ex);
                        MessageBox.Show(_localizer.Get("ErrorCreatingTemplate", ex.Message), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!SaveChangesDialog(e))
            {
                return;
            }

            base.OnFormClosing(e);
        }

        #endregion

        #region Cell Validations

        private void InitializeGridValidations()
        {
            // Data Grid Validation handlers
            dgvClientes.DataBindingComplete -= DgvClientes_DataBindingComplete;
            dgvClientes.DataBindingComplete += DgvClientes_DataBindingComplete;

            dgvClientes.CellEndEdit -= DgvClientes_CellEndEdit;
            dgvClientes.CellEndEdit += DgvClientes_CellEndEdit;

            // Initial Highlighting
            ApplyValidationHighlighting();
        }

        private void DgvClientes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ApplyValidationHighlighting();
        }

        private void DgvClientes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Revalidar solo la fila editada
            if (e.RowIndex < 0 || e.RowIndex >= dgvClientes.Rows.Count) return;
            var row = dgvClientes.Rows[e.RowIndex];
            ApplyValidationHighlightingForRow(row);
        }

        private void ApplyValidationHighlighting()
        {
            foreach (DataGridViewRow row in dgvClientes.Rows)
            {
                ApplyValidationHighlightingForRow(row);
            }
        }

        private void ApplyValidationHighlightingForRow(DataGridViewRow row)
        {
            // Resetear estilos de fila
            foreach (DataGridViewCell c in row.Cells)
            {
                c.Style.BackColor = Color.White;
                c.ToolTipText = string.Empty;
            }

            var cliente = row.DataBoundItem as Cliente;
            if (cliente == null) return;

            var results = ClienteValidator.GetValidationResults(cliente);
            if (results == null || results.Count == 0) return;

            foreach (var vr in results)
            {
                // Si no se especifican miembros, colorear toda la fila
                var members = vr.MemberNames?.ToList() ?? new List<string>();
                if (members.Count == 0)
                {
                    foreach (DataGridViewCell c in row.Cells)
                    {
                        c.Style.BackColor = Color.LightCoral;
                        // Append mensaje en tooltip
                        c.ToolTipText = string.IsNullOrEmpty(c.ToolTipText) ? vr.ErrorMessage : c.ToolTipText + "; " + vr.ErrorMessage;
                    }
                    continue;
                }

                foreach (var member in members)
                {
                    // Buscar columna que esté enlazada a la propiedad (DataPropertyName) o cuyo Name coincida
                    var col = dgvClientes.Columns
                        .Cast<DataGridViewColumn>()
                        .FirstOrDefault(c => string.Equals(c.DataPropertyName, member, StringComparison.OrdinalIgnoreCase)
                                             || string.Equals(c.Name, member, StringComparison.OrdinalIgnoreCase)
                                             || string.Equals(c.HeaderText, member, StringComparison.OrdinalIgnoreCase));

                    if (col != null && col.Index >= 0 && col.Index < row.Cells.Count)
                    {
                        var cell = row.Cells[col.Index];
                        cell.Style.BackColor = Color.LightCoral;
                        cell.ToolTipText = string.IsNullOrEmpty(cell.ToolTipText) ? vr.ErrorMessage : cell.ToolTipText + "; " + vr.ErrorMessage;
                    }
                }
            }
        }

        #endregion

        #region Auxiliary Methods

        private string GetFileFilterExtensions()
        {
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            if (fileExtensions == null || fileExtensions.Count == 0) return string.Empty;

            var parts = new List<string>();
            foreach (var ext in fileExtensions)
            {
                var extWithoutDot = ext.TrimStart('.').ToUpperInvariant();
                var label = _localizer.Get("FilesLabel");
                label = string.Format(label, extWithoutDot);
                parts.Add($"{label}|*{ext}");
            }

            return string.Join("|", parts);
        }

        private bool SaveChangesDialog(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                _localizer.Get("SaveChangesMessage"),
                _localizer.Get("SaveChangesTitle"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return false;
            }

            if (result == DialogResult.Yes)
            {
                try
                {
                    _storageService.Save(_clientes);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(_localizer.Get("ErrorSavingData"), ex);
                    MessageBox.Show(_localizer.Get("ErrorSavingData", ex.Message), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
