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

        public void Save(IEnumerable<Cliente> clientes)
        {
            string json = JsonConvert.SerializeObject(clientes, Formatting.Indented);
            File.WriteAllText(_localDbPath, json);
        }

        public List<Cliente> Load()
        {
            if (!File.Exists(_localDbPath)) return new List<Cliente>();

            string json = File.ReadAllText(_localDbPath);
            return JsonConvert.DeserializeObject<List<Cliente>>(json) ?? new List<Cliente>();
        }
    }
}
