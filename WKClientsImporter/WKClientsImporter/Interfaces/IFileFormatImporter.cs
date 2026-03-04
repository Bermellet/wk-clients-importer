using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using WKClientsImporter.Models;

namespace WKClientsImporter.Interfaces
{
    public interface IFileFormatImporter : IDataImporter
    {
        bool CanImport(string filePath);
    }
}