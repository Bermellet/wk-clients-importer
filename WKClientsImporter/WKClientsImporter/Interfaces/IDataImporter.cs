using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WKClientsImporter.Models;

namespace WKClientsImporter.Interfaces
{
    public interface IDataImporter
    {
        Task<List<Customer>> ImportAsync(string filePath, IProgress<int> progress);
    }
}
