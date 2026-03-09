using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class FileImporterService : IDataImporter
    {
        private readonly IEnumerable<IFileFormatImporter> _importers;

        public FileImporterService(IEnumerable<IFileFormatImporter> importers)
        {
            _importers = importers ?? throw new ArgumentNullException(nameof(importers));
        }

        public List<string> GetSupportedFileExtensions()
        {
            // Devolver extensiones soportadas para el modelo Cliente
            return _importers
                .Where(i => i.ModelType == typeof(Cliente))
                .Select(i => i.FileExtension)
                .ToList();
        }

        public async Task<List<Cliente>> ImportAsync(string filePath, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required", nameof(filePath));

            var importer = _importers
                .FirstOrDefault(i => i.ModelType == typeof(Cliente) && i.CanImport(filePath));

            if (importer == null)
            {
                var ext = Path.GetExtension(filePath);
                throw new NotSupportedException($"No importer registered for files with extension '{ext}' for model 'Cliente'");
            }

            var objects = await importer.ImportAsync(filePath, progress).ConfigureAwait(false);
            // Convertir a List<Cliente> y lanzar si hay incompatibilidades de tipo
            try
            {
                return objects.Cast<Cliente>().ToList();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException("The selected importer returned items that cannot be cast to Cliente.", ex);
            }
        }
    }
}