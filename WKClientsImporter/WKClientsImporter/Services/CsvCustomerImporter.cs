using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class CsvCustomerImporter : IDataImporter
    {
        public async Task<List<Customer>> ImportAsync(string filePath, IProgress<int> progress)
        {
            return await Task.Run(() =>
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    var records = new List<Customer>();
                    // Feedback del progreso
                    int total = File.ReadLines(filePath).Count();
                    int current = 0;

                    while (csv.Read())
                    {
                        records.Add(csv.GetRecord<Customer>());
                        current++;
                        progress?.Report((current * 100) / total);
                    }
                    return records;
                }
            });
        }
    }
}
