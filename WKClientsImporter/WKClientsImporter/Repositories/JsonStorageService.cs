using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class JsonStorageService : IStorageService
    {
        private readonly string _localDbPath = "data/clientes_store.json";

        public void Save(IEnumerable<Cliente> clientes)
        {
            string json = JsonConvert.SerializeObject(clientes, Formatting.Indented);

            CreateFolderIfNotExist(_localDbPath);
            File.WriteAllText(_localDbPath, json);
        }

        private void CreateFolderIfNotExist(string path)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dir = Path.GetDirectoryName(path);
            var directory = Path.Combine(baseDir, dir);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public List<Cliente> Load()
        {
            if (!File.Exists(_localDbPath)) return new List<Cliente>();

            string json = File.ReadAllText(_localDbPath);
            return JsonConvert.DeserializeObject<List<Cliente>>(json) ?? new List<Cliente>();
        }
    }
}
