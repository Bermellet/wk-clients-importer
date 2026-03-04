using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;

namespace WKClientsImporter.Services
{
    public class TemplateBuilderService : ITemplateBuilder
    {
        public async Task BuildCsvTemplateAsync(string filePath)
        {
            ValidatePath(filePath);

            await Task.Run(() =>
            {
                var props = typeof(Customer)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToArray();

                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Delimiter for excel
                    csv.WriteField("sep=,");
                    csv.NextRecord();

                    // Headers
                    foreach (var prop in props)
                    {
                        csv.WriteField(prop.Name);
                    }
                    csv.NextRecord();

                    // Hints validation
                    //foreach (var prop in props)
                    //{
                    //    StringBuilder hints = GenerateFieldHints(prop);
                    //    csv.WriteField(hints.ToString());
                    //}
                    //csv.NextRecord();
                }
            });
        }


        public async Task BuildJsonTemplateAsync(string filePath)
        {
            ValidatePath(filePath);

            await Task.Run(() =>
            {
                var props = typeof(Customer)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToArray();

                var templateObj = new JObject();
                //var hintsObj = new JObject();

                foreach (var prop in props)
                {
                    templateObj[prop.Name] = JValue.CreateString(string.Empty);
                    //StringBuilder hints = GenerateFieldHints(prop);
                    //hintsObj[prop.Name] = JValue.CreateString(hints.ToString());
                }

                var root = new JArray
                {
                    templateObj,
                    //["hints"] = hintsObj
                };

                File.WriteAllText(filePath, root.ToString(Formatting.Indented), Encoding.UTF8);
            });
        }

        private void ValidatePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path doesn't exist", nameof(filePath));
            }

            // Aseguramos directorio
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        private StringBuilder GenerateFieldHints(PropertyInfo prop)
        {
            var hints = new StringBuilder();

            var required = prop.GetCustomAttribute<RequiredAttribute>();
            if (required != null)
            {
                hints.Append("REQUIRED");
            }

            var strLen = prop.GetCustomAttribute<StringLengthAttribute>();
            if (strLen != null)
            {
                if (hints.Length > 0) hints.Append("; ");
                hints.Append($"MaxLength={strLen.MaximumLength}");
            }

            var regex = prop.GetCustomAttribute<RegularExpressionAttribute>();
            if (regex != null)
            {
                if (hints.Length > 0) hints.Append("; ");
                hints.Append($"Regex={regex.Pattern}");
            }

            var email = prop.GetCustomAttribute<EmailAddressAttribute>();
            if (email != null)
            {
                if (hints.Length > 0) hints.Append("; ");
                hints.Append("Email");
            }

            var dataType = prop.GetCustomAttribute<DataTypeAttribute>();
            if (dataType != null)
            {
                if (hints.Length > 0) hints.Append("; ");
                hints.Append($"DataType={dataType.DataType}");
            }

            var custom = prop.GetCustomAttribute<CustomValidationAttribute>();
            if (custom != null)
            {
                if (hints.Length > 0) hints.Append("; ");
                hints.Append($"CustomValidation={custom.Method}");
            }

            return hints;
        }
    }
}
