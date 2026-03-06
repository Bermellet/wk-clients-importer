using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Views
{
    public partial class MainForm : Form
    {
        private BindingList<Cliente> _clientes;
        private readonly IStorageService _storageService;
        private readonly IDataImporter _importerService;
        private readonly ITemplateBuilder _templateBuilder;

        public MainForm(IStorageService storageService, IDataImporter importerService,
            ITemplateBuilder templateBuilder)
        {
            InitializeComponent();
            _storageService = storageService;
            _importerService = importerService;
            _templateBuilder = templateBuilder;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            var data = _storageService.Load() ?? new List<Cliente>();
            _clientes = new BindingList<Cliente>(data);
            dgvClientes.DataSource = _clientes;
        }

        private async void btnImport_Click(object sender, EventArgs e)
        {
            var filterExtensions = GetFileFilterExtensions();
            OpenFileDialog dialog = new OpenFileDialog { Filter = filterExtensions };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var progress = new Progress<int>(v => pbImport.Value = v);

                try
                {
                    var importedData = await _importerService.ImportAsync(dialog.FileName, progress);

                    foreach (var item in importedData)
                    {
                        _clientes.Add(item);
                    }

                    pbImport.Value = 0; // Reset
                    MessageBox.Show($"{importedData.Count} clientes have been imported");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing: {ex.Message}");
                }
            }
        }

        private string GetFileFilterExtensions()
        {
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            var filter = string.Join("|", fileExtensions.ConvertAll(ext => $"{ext.TrimStart('.').ToUpper()} Files|*{ext}"));
            return filter;
        }

        private async void btnTemplate_Click(object sender, EventArgs e)
        {
            // Obtener extensiones soportadas por el servicio
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            if (fileExtensions == null || fileExtensions.Count == 0)
            {
                MessageBox.Show("No supported file formats available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mostrar diálogo para que el usuario elija el formato
            using (var formatDialog = new TemplateFormatDialog(fileExtensions))
            {
                if (formatDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var selectedExt = formatDialog.SelectedExtension; // ejemplo ".csv" o ".json"
                if (string.IsNullOrWhiteSpace(selectedExt))
                {
                    MessageBox.Show("No format selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Construir filtro simple para SaveFileDialog basado en la extensión seleccionada
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
                        // Informar al servicio del formato elegido; el servicio decide qué motor usar.
                        await _templateBuilder.BuildTemplateAsync(dialog.FileName, selectedExt);
                        MessageBox.Show("Template correctly generated", "Template created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating template: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool SaveChangesDialog(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Do you want to save changes before closing?",
                "Save changes",
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
                    MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Cancel close so user can retry or investigate
                    e.Cancel = true;
                    return false;
                }
            }

            return true;
        }
    }
}
