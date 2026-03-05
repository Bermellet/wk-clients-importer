using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Views
{
    public partial class MainForm : Form
    {
        private BindingList<Customer> _customers;
        private readonly IDataImporter _importerService;
        private readonly IStorageService _storageService;
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

        // TODO: Data validations
        // TODO: Avoid blocking Task.Run, IProgress<T> ?
        private async void btnImportCsv_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "CSV Files|*.csv" }; // TODO; Single file, get extensions from service

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var progress = new Progress<int>(v => pbImport.Value = v);

                try
                {
                    var importedData = await _importerService.ImportAsync(dialog.FileName, progress);

                    // Actualizamos BindingList (la UI se refresca sola)
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

        private async void btnImportJson_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "JSON Files|*.json" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var progress = new Progress<int>(v => pbImport.Value = v);

                try
                {
                    var importedData = await _importerService.ImportAsync(dialog.FileName, progress);

                    // Actualizamos BindingList (la UI se refresca sola)
                    foreach (var customer in importedData)
                    {
                        _customers.Add(customer);
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

        private async void btnCsvTemplate_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "CSV Files|*.csv", FileName = "clients_template.csv" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await _templateBuilder.BuildCsvTemplateAsync(dialog.FileName);
                        MessageBox.Show("CSV template correctly generated", "Template created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating CSV template: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnJsonTemplate_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "JSON Files|*.json", FileName = "clients_template.json" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await _templateBuilder.BuildJsonTemplateAsync(dialog.FileName);
                        MessageBox.Show("JSON template correctly generated", "Template created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating JSON template: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
