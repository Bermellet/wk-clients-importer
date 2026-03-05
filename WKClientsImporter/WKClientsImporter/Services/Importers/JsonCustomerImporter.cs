using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class JsonCustomerImporter : IFileFormatImporter
    {
        public string FileExtension { get => ".json"; }
        public List<string> GetSupportedFileExtensions() => new List<string> { FileExtension };

        public bool CanImport(string filePath)
        {
            return string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        // Importación optimizada: parseo en streaming para evitar cargar todo el JSON en memoria.
        // Se delega el trabajo de parseo a un hilo de fondo (Task.Run) pero sin materializar antes
        // un JToken completo cuando el JSON tiene un array de elementos en la raíz.
        public async Task<List<Customer>> ImportAsync(string filePath, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            // Abrir FileStream en modo asíncrono para permitir lecturas sin bloquear el hilo llamador.
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            using (var reader = new JsonTextReader(sr))
            {
                long totalBytes = fs.Length;

                return await Task.Run(() =>
                {
                    var customers = new List<Customer>();
                    var serializer = new JsonSerializer();

                    try
                    {
                        // Buscar un array en la raíz y, si lo hay, procesarlo en streaming.
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                // Procesamos los elementos del array uno a uno.
                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonToken.StartObject)
                                    {
                                        var cust = serializer.Deserialize<Customer>(reader);
                                        customers.Add(cust);

                                        if (totalBytes > 0)
                                        {
                                            int percentage = (int)((sr.BaseStream.Position * 100) / totalBytes);
                                            progress?.Report(Math.Min(percentage, 100));
                                        }
                                    }
                                    else if (reader.TokenType == JsonToken.EndArray)
                                    {
                                        break;
                                    }
                                }
                                return customers;
                            }
                        }

                        // Si no encontramos un array en la raíz, reiniciamos el stream y hacemos una estrategia de fallback:
                        // leemos el documento completo (JToken) pero esto sólo ocurre si el JSON no es un array en la raíz,
                        // así evitamos la carga en memoria para los escenarios comunes (arrays grandes).
                        fs.Seek(0, SeekOrigin.Begin);
                        sr.DiscardBufferedData();
                        using (var reader2 = new JsonTextReader(sr))
                        {
                            var token = JToken.ReadFrom(reader2);

                            if (token.Type == JTokenType.Array)
                            {
                                foreach (var t in token.Children())
                                {
                                    customers.Add(t.ToObject<Customer>());
                                }
                            }
                            else if (token.Type == JTokenType.Object)
                            {
                                var obj = (JObject)token;
                                var arrayProp = obj.Properties().FirstOrDefault(p => p.Value.Type == JTokenType.Array);
                                if (arrayProp != null)
                                {
                                    foreach (var t in arrayProp.Value.Children())
                                    {
                                        customers.Add(t.ToObject<Customer>());
                                    }
                                }
                                else
                                {
                                    // Documento representando un único cliente
                                    customers.Add(token.ToObject<Customer>());
                                }
                            }
                        }

                        return customers;
                    }
                    catch
                    {
                        // Re-lanzar la excepción al llamador para manejo de UI / logging
                        throw;
                    }
                }).ConfigureAwait(false);
            }
        }

        // Método existente: mantiene un modo de deserialización por streaming basado en JsonTextReader,
        // se puede dejar para escenarios alternativos. Se podría unificar con ImportAsync si se desea.
        public async Task<List<Customer>> ImportAsyncBytes(string filePath, IProgress<int> progress)
        {
            return await Task.Run(() =>
            {
                var customers = new List<Customer>();
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(filePath))
                using (var reader = new JsonTextReader(sr))
                {
                    long totalBytes = sr.BaseStream.Length;

                    // Avanzamos hasta el inicio del array
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.StartArray) break;
                    }

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.EndArray) break;

                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var customer = serializer.Deserialize<Customer>(reader);
                            customers.Add(customer);

                            if (totalBytes > 0)
                            {
                                int percentage = (int)((sr.BaseStream.Position * 100) / totalBytes);
                                progress?.Report(Math.Min(percentage, 100));
                            }
                        }
                    }
                }
                return customers;
            });
        }
    }
}
