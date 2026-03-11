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

        async Task<IEnumerable<object>> IFileFormatImporter.ImportAsync(string filePath, IProgress<int> progress)
        {
            var list = await ImportAsync(filePath, progress).ConfigureAwait(false);
            return list.Cast<object>();
        }

        public async Task<List<TModel>> ImportAsync(string filePath, IProgress<int> progress)
        {
            return await Task.Run(() =>
            {
                using (var reader = new StreamReader(filePath))
                {
                    // Leer la primera línea para detectar "sep=" o para inferir delimitador
                    string firstLine = reader.ReadLine();
                    if (firstLine == null) return new List<TModel>();

                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { MissingFieldFound = null };
                    bool hasSep = firstLine.StartsWith("sep=", StringComparison.OrdinalIgnoreCase);
                    if (hasSep)
                    {
                        string delimiter = firstLine.Substring(4);
                        config.Delimiter = delimiter;
                    }
                    else
                    {
                        string delimiter = CheckDelimiterByFirstLine(firstLine);
                        config.Delimiter = delimiter;

                        // Volver a la posición inicial para que CsvReader lea la cabecera correcta
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        reader.DiscardBufferedData();
                    }

                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Read();
                        csv.ReadHeader();

                        var records = new List<TModel>();
                        int totalLines = File.ReadLines(filePath).Count();
                        int headerLines = hasSep ? 2 : 1; // sep= + header, o solo header
                        int total = Math.Max(1, totalLines - headerLines);
                        int current = 0;

                        while (csv.Read())
                        {
                            var record = csv.GetRecord<TModel>();
                            records.Add(record);
                            current++;
                            progress?.Report((current * 100) / total);
                        }
                        progress?.Report(100);
                        return records;
                    }
                }
            });
        }

        private string CheckDelimiterByFirstLine(string firstLine)
        {
            // Detectar delimitador probando varios candidatos y contando ocurrencias fuera de comillas
            string[] candidates = { ",", ";", "\t", "|", ":" };
            string chosen = ",";
            int maxCount = -1;

            foreach (var c in candidates)
            {
                int count = CountDelimiterOccurrencesInCsvHeader(firstLine, c);
                if (count > maxCount)
                {
                    maxCount = count;
                    chosen = c;
                }
            }

            return chosen;
        }

        private int CountDelimiterOccurrencesInCsvHeader(string line, string delimiter)
        {
            if (string.IsNullOrEmpty(line)) return 0;
            if (delimiter == null) return 0;

            // Contar ocurrencias del delimitador solo fuera de campos entrecomillados
            int count = 0;
            bool inQuotes = false;

            // delimitadores considerados son de 1 carácter en nuestra lista (incluye '\t')
            char d = delimiter.Length > 0 ? delimiter[0] : '\0';

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (!inQuotes && ch == d)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
