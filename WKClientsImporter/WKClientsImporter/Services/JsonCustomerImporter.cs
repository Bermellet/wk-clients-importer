using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class JsonCustomerImporter : IDataImporter
    {
        public async Task<List<Customer>> ImportAsync(string filePath, IProgress<int> progress)
        {
            return await Task.Run(() =>
            {
                using (var reader = new StreamReader(filePath))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var token = JToken.ReadFrom(jsonReader);

                    // Normalizar a lista de JToken que representan clientes
                    List<JToken> items;
                    if (token.Type == JTokenType.Array)
                    {
                        items = token.Children().ToList();
                    }
                    else if (token.Type == JTokenType.Object)
                    {
                        var obj = (JObject)token;
                        // Buscar una propiedad que contenga un array (p. ej. { "customers": [ ... ] })
                        var arrayProp = obj.Properties().FirstOrDefault(p => p.Value.Type == JTokenType.Array);
                        if (arrayProp != null)
                        {
                            items = arrayProp.Value.Children().ToList();
                        }
                        else
                        {
                            // Tratar el objeto entero como un único registro
                            items = new List<JToken> { token };
                        }
                    }
                    else
                    {
                        items = new List<JToken>();
                    }

                    var records = new List<Customer>();
                    // Feedback del progreso
                    int total = items.Count;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var customer = items[i].ToObject<Customer>();
                        records.Add(customer);
                        progress?.Report(((i + 1) * 100) / total);
                    }

                    return records;
                }
            });
        }
    }
}
