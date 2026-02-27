using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class JsonStorageService : IStorageService
    {
        public void Save(IEnumerable<Customer> customers, string filePath)
        {
            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public List<Customer> Load(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Customer>();

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
        }
    }
}
