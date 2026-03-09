using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;

namespace WKClientsImporter.Services
{
    public class JsonFileImporter<TModel> : IFileFormatImporter<TModel>
    {
        public string FileExtension => ".json";
        public Type ModelType => typeof(TModel);

        public bool CanImport(string filePath)
        {
            return string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<TModel>> ImportAsync(string filePath, IProgress<int> progress)
        {
            return await Task.Run(() =>
            {
                using (var reader = new StreamReader(filePath))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var token = JToken.ReadFrom(jsonReader);

                    List<JToken> items;
                    if (token.Type == JTokenType.Array)
                    {
                        items = token.Children().ToList();
                    }
                    else if (token.Type == JTokenType.Object)
                    {
                        var obj = (JObject)token;
                        var arrayProp = obj.Properties().FirstOrDefault(p => p.Value.Type == JTokenType.Array);
                        if (arrayProp != null)
                        {
                            items = arrayProp.Value.Children().ToList();
                        }
                        else
                        {
                            items = new List<JToken> { token };
                        }
                    }
                    else
                    {
                        items = new List<JToken>();
                    }

                    var records = new List<TModel>();
                    int total = items.Count;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var model = items[i].ToObject<TModel>();
                        records.Add(model);
                        progress?.Report(total == 0 ? 100 : ((i + 1) * 100) / total);
                    }

                    return records;
                }
            });
        }

        async Task<IEnumerable<object>> IFileFormatImporter.ImportAsync(string filePath, IProgress<int> progress)
        {
            var list = await ImportAsync(filePath, progress).ConfigureAwait(false);
            return list.Cast<object>();
        }
    }
}
