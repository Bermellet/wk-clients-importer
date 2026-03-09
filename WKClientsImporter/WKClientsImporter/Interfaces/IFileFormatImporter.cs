using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WKClientsImporter.Interfaces
{
    public interface IFileFormatImporter
    {
        string FileExtension { get; }
        Type ModelType { get; }
        bool CanImport(string filePath);
        Task<IEnumerable<object>> ImportAsync(string filePath, IProgress<int> progress);
    }
}