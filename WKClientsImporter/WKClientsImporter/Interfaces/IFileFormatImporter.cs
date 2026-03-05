
namespace WKClientsImporter.Interfaces
{
    public interface IFileFormatImporter : IDataImporter
    {
        string FileExtension { get; }
        bool CanImport(string filePath);
    }
}