using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;
using WKClientsImporter.Services;

namespace WKClientsImporter.Views
{
    public partial class MainForm : Form
    {
        private BindingList<Customer> _customers;
        private readonly IStorageService _storageService;
        private readonly IDataImporter _csvImporterService;
        private readonly IDataImporter _jsonImporterService;
        private readonly string _localDbPath = "clientes_store.json";

        public MainForm()
        {
            InitializeComponent();
            _storageService = new JsonStorageService(); // TODO: Inyección de Dependencias
            _csvImporterService = new CsvCustomerImporter(); // TODO: Inyección de Dependencias
            _jsonImporterService = new JsonCustomerImporter(); // TODO: Inyección de Dependencias
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            var data = _storageService.Load(_localDbPath) ?? new List<Customer>();
            _customers = new BindingList<Customer>(data);
            dgvCustomers.DataSource = _customers;
        }

        // TODO: Data validations
        // TODO: Avoid blocking Task.Run, IProgress<T> ?
        private async void btnImportCsv_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "CSV Files|*.csv" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var progress = new Progress<int>(v => pbImport.Value = v);

                try
                {
                    var importedData = await _csvImporterService.ImportAsync(dialog.FileName, progress);

                    // Actualizamos BindingList (la UI se refresca sola)
                    foreach (var item in importedData)
                    {
                        _customers.Add(item);
                    }

                    pbImport.Value = 0; // Reset
                    MessageBox.Show($"Se han importado {importedData.Count} clientes.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al importar: {ex.Message}");
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
                    var importedData = await _jsonImporterService.ImportAsync(dialog.FileName, progress);

                    // Actualizamos BindingList (la UI se refresca sola)
                    foreach (var customer in importedData)
                    {
                        _customers.Add(customer);
                    }

                    pbImport.Value = 0; // Reset
                    MessageBox.Show($"Se han importado {importedData.Count} clientes.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al importar: {ex.Message}");
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _storageService.Save(_customers, _localDbPath);
            base.OnFormClosing(e);
        }
    }
}
