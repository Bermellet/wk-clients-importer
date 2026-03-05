using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class CsvCustomerImporter : IFileFormatImporter
    {
        public string FileExtension { get => ".csv"; }
        public List<string> GetSupportedFileExtensions() => new List<string> { FileExtension };

        public bool CanImport(string filePath)
        {
            return string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<Customer>> ImportAsync(string filePath, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            // Abrir el stream en modo asíncrono
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                // Detectar línea "sep=," si existe y ajustar el delimitador antes de construir CsvReader
                string firstLine = await sr.ReadLineAsync().ConfigureAwait(false);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Si falta un campo, preferimos reportarlo en validación en vez de lanzar aquí; ajustar según necesidad
                    MissingFieldFound = null,
                    BadDataFound = null,
                    PrepareHeaderForMatch = args => args.Header // mantener nombres tal cual
                };

                if (!string.IsNullOrEmpty(firstLine) && firstLine.StartsWith("sep=", StringComparison.OrdinalIgnoreCase))
                {
                    var delim = firstLine.Substring("sep=".Length);
                    if (!string.IsNullOrEmpty(delim))
                    {
                        config.Delimiter = delim;
                    }
                }
                else
                {
                    // Volver al inicio si la primera línea no era la linea sep=
                    fs.Seek(0, SeekOrigin.Begin);
                    sr.DiscardBufferedData();
                }

                using (var csv = new CsvReader(sr, config))
                {
                    // Leer cabecera (métodos async disponibles según versión de CsvHelper)
                    if (!await csv.ReadAsync().ConfigureAwait(false))
                        return new List<Customer>();

                    csv.ReadHeader();

                    var records = new List<Customer>();
                    long totalBytes = fs.Length;
                    int lastReported = -1;

                    while (await csv.ReadAsync().ConfigureAwait(false))
                    {
                        // Mapear registro actual (GetRecord es síncrono pero ligero)
                        var record = csv.GetRecord<Customer>();
                        records.Add(record);

                        // Reportar progreso aproximado por bytes leídos (estimación)
                        if (totalBytes > 0)
                        {
                            var pos = fs.Position;
                            int pct = (int)((pos * 100L) / totalBytes);
                            // Reducir envío de reportes redundantes
                            if (pct != lastReported)
                            {
                                lastReported = pct;
                                progress?.Report(Math.Min(Math.Max(pct, 0), 100));
                            }
                        }
                    }

                    // Asegurar 100% al final
                    progress?.Report(100);
                    return records;
                }
            }
        }
    }
}
