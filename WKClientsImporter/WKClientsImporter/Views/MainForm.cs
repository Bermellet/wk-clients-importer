using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Localization;
using WKClientsImporter.Models;

namespace WKClientsImporter.Views
{
    public partial class MainForm : Form
    {
        private BindingList<Cliente> _clientes;
        private readonly IStorageService _storageService;
        private readonly IDataImporter _importerService;
        private readonly ITemplateBuilder _templateBuilder;
        private readonly IStringLocalizer _localizer;

        public MainForm(IStorageService storageService, IDataImporter importerService,
            ITemplateBuilder templateBuilder, IStringLocalizer localizer)
        {
            InitializeComponent();
            _storageService = storageService;
            _importerService = importerService;
            _templateBuilder = templateBuilder;
            _localizer = localizer;
            ApplyLocalizer();
            LoadInitialData();
        }

        private void ApplyLocalizer()
        {
            this.Text = _localizer.Get("MainFormTitle");
            btnImport.Text = _localizer.Get("ButtonImport");
            btnTemplate.Text = _localizer.Get("ButtonTemplate");
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
                    MessageBox.Show(_localizer.Get("ImportedCount", importedData.Count), _localizer.Get("InformationTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_localizer.Get("ErrorImporting", ex.Message), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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

        private async void btnTemplate_Click(object sender, EventArgs e)
        {
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            if (fileExtensions == null || fileExtensions.Count == 0)
            {
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
                        MessageBox.Show(_localizer.Get("TemplateCreated"), _localizer.Get("TemplateCreatedTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
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
                    MessageBox.Show(_localizer.Get("ErrorSavingData", ex.Message), _localizer.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return false;
                }
            }

            return true;
        }
    }
}
