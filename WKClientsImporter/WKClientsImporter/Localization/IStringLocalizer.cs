
namespace WKClientsImporter.Localization
{
    public interface IStringLocalizer
    {
        string Get(string key);
        string Get(string key, params object[] args);
    }
}