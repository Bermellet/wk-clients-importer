using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;

namespace WKClientsImporter.Services
{
    public class CsvFileImporter<TModel> : IFileFormatImporter<TModel>
    {
        public string FileExtension => ".csv";
        public Type ModelType => typeof(TModel);

        public List<string> GetSupportedFileExtensions() => new List<string> { FileExtension };

        public bool CanImport(string filePath)
        {
            return string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<TModel>> ImportAsync(string filePath, IProgress<int> progress)
        {
            return await Task.Run(() =>
            {
                using (var reader = new StreamReader(filePath))
                {
                    // Leer la primera línea para detectar "sep="
                    string firstLine = reader.ReadLine();
                    string delimiter = ",";

                    if (firstLine != null && firstLine.StartsWith("sep="))
                    {
                        delimiter = firstLine.Substring(4);
                    }

                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = delimiter, MissingFieldFound = null };
                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Read();
                        csv.ReadHeader();

                        var records = new List<TModel>();
                        int total = File.ReadLines(filePath).Count();
                        if (firstLine != null && firstLine.StartsWith("sep=")) total--; // ajustar progreso si había sep=
                        int current = 0;

                        while (csv.Read())
                        {
                            var record = csv.GetRecord<TModel>();
                            records.Add(record);
                            current++;
                            progress?.Report((current * 100) / Math.Max(1, total));
                        }
                        return records;
                    }
                }
            });
        }

        async Task<IEnumerable<object>> IFileFormatImporter.ImportAsync(string filePath, IProgress<int> progress)
        {
            var list = await ImportAsync(filePath, progress).ConfigureAwait(false);
            return list.Cast<object>();
        }
    }
}
