using System.Collections.Generic;
using WKClientsImporter.Models;

namespace WKClientsImporter.Interfaces
{
    public interface IStorageService
    {
        void Save(IEnumerable<Customer> customers, string filePath);
        List<Customer> Load(string filePath);
    }
}
