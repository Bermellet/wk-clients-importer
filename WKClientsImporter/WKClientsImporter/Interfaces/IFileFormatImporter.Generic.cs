using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WKClientsImporter.Interfaces
{
    public interface IFileFormatImporter<TModel> : IFileFormatImporter
    {
        Task<List<TModel>> ImportAsync(string filePath, IProgress<int> progress);
    }
}