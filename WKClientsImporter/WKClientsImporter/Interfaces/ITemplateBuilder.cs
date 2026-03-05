using System.Threading.Tasks;

namespace WKClientsImporter.Interfaces
{
    public interface ITemplateBuilder
    {
        Task BuildTemplateAsync(string filePath, string extension);
    }
}
