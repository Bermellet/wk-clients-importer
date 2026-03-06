using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
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
        public Task BuildTemplateAsync(string filePath, string extension)
        {
            // TODO: Optimize distinguishing by extension, maybe using a dictionary of builders
            if (string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BuildCsvTemplateAsync(filePath);
            }
            else if (string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
            {
                return BuildJsonTemplateAsync(filePath);
            }
            else
            {
                throw new NotSupportedException($"Unsupported template format '{extension}'");
            }
        }

        private async Task BuildCsvTemplateAsync(string filePath)
        {
            ValidatePath(filePath);

            var props = typeof(Cliente)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            // Abrir FileStream en modo asíncrono y usar StreamWriter.WriteAsync para no bloquear
            var dir = Path.GetDirectoryName(filePath);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                // Línea de delimitador para Excel
                await writer.WriteLineAsync("sep=,").ConfigureAwait(false);

                // Cabeceras (CSV simple, sin escapar ya que son nombres de propiedades)
                var header = string.Join(",", props.Select(p => p.Name));
                await writer.WriteLineAsync(header).ConfigureAwait(false);

                // No añadimos filas de ejemplo para mantener low-memory y simplicidad.
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private async Task BuildJsonTemplateAsync(string filePath)
        {
            ValidatePath(filePath);

            var props = typeof(Cliente)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            // Construimos el objeto en memoria (pequeño: sólo propiedades) y lo serializamos de forma asíncrona
            var templateObj = new JObject();
            foreach (var prop in props)
            {
                templateObj[prop.Name] = JValue.CreateString(string.Empty);
            }

            var root = new JArray { templateObj };

            // Serializar a string y escribir asíncronamente usando FileStream/StreamWriter
            var json = root.ToString(Formatting.Indented);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                await writer.WriteAsync(json).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
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
