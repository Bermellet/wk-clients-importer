using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace WKClientsImporter.Localization
{
    public class JsonFileStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, string> _strings;

        public JsonFileStringLocalizer()
        {
            _strings = LoadStringsForCurrentCulture();
        }

        private Dictionary<string, string> LoadStringsForCurrentCulture()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Determinar lenguaje (ej: "es" o "en")
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName ?? "en";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var relPath = Path.Combine("Localization", $"{lang}.json");
            var fallbackPath = Path.Combine("Localization", "en.json");

            string path = File.Exists(Path.Combine(baseDir, relPath)) ? Path.Combine(baseDir, relPath)
                         : (File.Exists(Path.Combine(baseDir, fallbackPath)) ? Path.Combine(baseDir, fallbackPath) : null);

            if (path == null) return dict;

            try
            {
                var json = File.ReadAllText(path);
                var jobj = JObject.Parse(json);
                foreach (var prop in jobj.Properties())
                {
                    dict[prop.Name] = prop.Value.ToString();
                }
            }
            catch
            {
                // si falla, devolver diccionario vacío para que Get devuelva la clave
            }

            return dict;
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;
            if (_strings.TryGetValue(key, out var val)) return val;
            return key; // fallback: devolver la clave para detectar strings faltantes
        }

        public string Get(string key, params object[] args)
        {
            var template = Get(key);
            return args == null || args.Length == 0 ? template : string.Format(template, args);
        }
    }
}