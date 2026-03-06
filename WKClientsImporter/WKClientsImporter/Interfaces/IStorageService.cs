using System.Collections.Generic;
using WKClientsImporter.Models;

namespace WKClientsImporter.Interfaces
{
    public interface IStorageService
    {
        void Save(IEnumerable<Cliente> clientes);
        List<Cliente> Load();
    }
}
