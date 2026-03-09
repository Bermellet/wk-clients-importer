using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Models;
using WKClientsImporter.Models.Validators;

namespace WKClientsImporter.Services
{
    public class FileImporterService : IDataImporter
    {
        private readonly IEnumerable<IFileFormatImporter> _importers;
        private readonly ILogger _logger;

        public FileImporterService(IEnumerable<IFileFormatImporter> importers, ILogger logger)
        {
            _importers = importers ?? throw new ArgumentNullException(nameof(importers));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<string> GetSupportedFileExtensions()
        {
            // Devolver extensiones soportadas para el modelo Cliente
            return _importers
                .Where(i => i.ModelType == typeof(Cliente))
                .Select(i => i.FileExtension)
                .ToList();
        }

        public async Task<List<Cliente>> ImportAsync(string filePath, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required", nameof(filePath));

            var importer = _importers
                .FirstOrDefault(i => i.ModelType == typeof(Cliente) && i.CanImport(filePath));

            if (importer == null)
            {
                var ext = Path.GetExtension(filePath);
                throw new NotSupportedException($"No importer registered for files with extension '{ext}' for model 'Cliente'");
            }

            var objects = await importer.ImportAsync(filePath, progress).ConfigureAwait(false);
            // Convertir a List<Cliente> y manejar incompatibilidades de tipo
            try
            {
                var clientes = objects.Cast<Cliente>().ToList();

                var validClients = new List<Cliente>();
                int invalidCount = 0;

                for (int i = 0; i < clientes.Count; i++)
                {
                    var cliente = clientes[i];
                    if (!ClienteValidator.TryValidate(cliente, out List<string> errors))
                    {
                        invalidCount++;
                        var rowInfo = $"Fila {i + 1}";
                        var dni = string.IsNullOrWhiteSpace(cliente?.DNI) ? "DNI=N/A" : $"DNI={cliente.DNI}";
                        var message = $"{rowInfo} ({dni}): {string.Join("; ", errors)}";
                        // Loguear el registro erróneo y continuar
                        _logger?.LogWarning($"Registro inválido importado: {message}");
                        continue; // ignorar este registro, no añadir a validClients
                    }

                    validClients.Add(cliente);
                }

                _logger?.LogInfo($"Importación finalizada. Registros válidos: {validClients.Count}. Registros ignorados por error: {invalidCount}.");

                return validClients;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException("The selected importer returned items that cannot be cast to Cliente.", ex);
            }
        }
    }
}