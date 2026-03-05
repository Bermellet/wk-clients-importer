using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Views
{
    public partial class MainForm : Form
    {
        private BindingList<Customer> _customers;
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
            var data = _storageService.Load() ?? new List<Customer>();
            _customers = new BindingList<Customer>(data);
            dgvCustomers.DataSource = _customers;
        }

        private async void btnImport_Click(object sender, EventArgs e)
        {
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            var filter = string.Join("|", fileExtensions.ConvertAll(ext => $"{ext.TrimStart('.').ToUpper()} Files|*{ext}"));
            OpenFileDialog dialog = new OpenFileDialog { Filter = filter };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var progress = new Progress<int>(v => pbImport.Value = v);

                try
                {
                    var importedData = await _importerService.ImportAsync(dialog.FileName, progress);

                    foreach (var item in importedData)
                    {
                        _customers.Add(item);
                    }

                    pbImport.Value = 0; // Reset
                    MessageBox.Show($"{importedData.Count} customers have been imported");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing: {ex.Message}");
                }
            }
        }

        private async void btnTemplate_Click(object sender, EventArgs e)
        {
            var fileExtensions = _importerService.GetSupportedFileExtensions();
            if (fileExtensions == null || fileExtensions.Count == 0)
            {
                MessageBox.Show("No supported file formats available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var formatDialog = new TemplateFormatDialog(fileExtensions))
            {
                if (formatDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var selectedExt = formatDialog.SelectedExtension;
                if (string.IsNullOrWhiteSpace(selectedExt))
                {
                    MessageBox.Show("No format selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    _storageService.Save(_customers);
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
