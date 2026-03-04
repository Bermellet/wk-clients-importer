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

        public async Task<List<Customer>> ImportAsync(string filePath, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required", nameof(filePath));

            var importer = _importers.FirstOrDefault(i => i.CanImport(filePath));
            if (importer == null)
            {
                var ext = Path.GetExtension(filePath);
                throw new NotSupportedException($"No importer registered for files with extension '{ext}'");
            }

            return await importer.ImportAsync(filePath, progress);
        }
    }
}