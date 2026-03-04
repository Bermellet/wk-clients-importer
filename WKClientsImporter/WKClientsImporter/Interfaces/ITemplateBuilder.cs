using System.Threading.Tasks;

namespace WKClientsImporter.Interfaces
{
    public interface ITemplateBuilder
    {
        Task BuildCsvTemplateAsync(string filePath);
        Task BuildJsonTemplateAsync(string filePath);
    }
}
