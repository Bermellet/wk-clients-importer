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
        private readonly ILogger _logger;


        public FileImporterService(IEnumerable<IFileFormatImporter> importers, ILogger logger)
        {
            _importers = importers ?? throw new ArgumentNullException(nameof(importers));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<string> GetSupportedFileExtensions()
        {
            return _importers.Select(i => i.FileExtension).ToList();
        }

        public async Task<List<Cliente>> ImportAsync(string filePath, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required", nameof(filePath));

            var importer = _importers.FirstOrDefault(i => i.CanImport(filePath));
            if (importer == null)
            {
                var ext = Path.GetExtension(filePath);
                var errorMessage = $"No importer found for file '{filePath}' with extension '{ext}'";
                _logger.LogError(errorMessage);
                throw new NotSupportedException(errorMessage);
            }

            return await importer.ImportAsync(filePath, progress);
        }
    }
}