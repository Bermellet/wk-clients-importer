using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class JsonStorageService : IStorageService
    {
        private readonly string _localDbPath = "clients_store.json";

        public void Save(IEnumerable<Customer> customers)
        {
            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            File.WriteAllText(_localDbPath, json);
        }

        public List<Customer> Load()
        {
            if (!File.Exists(_localDbPath)) return new List<Customer>();

            string json = File.ReadAllText(_localDbPath);
            return JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
        }
    }
}
