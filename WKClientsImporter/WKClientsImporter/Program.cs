using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
using WKClientsImporter.Localization;
using WKClientsImporter.Models;
using WKClientsImporter.Services;
using WKClientsImporter.Views;

namespace WKClientsImporter
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Dependency Injection
            var services = new ServiceCollection();
            services.AddTransient<MainForm>();
            // Localization
            services.AddSingleton<IStringLocalizer, JsonFileStringLocalizer>();
            // Logger
            services.AddSingleton<ILogger, FileLogger>();
            // Repository and template
            services.AddSingleton<IStorageService, JsonStorageService>();
            services.AddSingleton<ITemplateBuilder, TemplateBuilderService>();
            // Importers
            services.AddSingleton<IFileFormatImporter>(sp => new CsvFileImporter<Cliente>());
            services.AddSingleton<IFileFormatImporter>(sp => new JsonFileImporter<Cliente>());
            services.AddSingleton<IDataImporter, FileImporterService>();

            var provider = services.BuildServiceProvider();
            Application.Run(provider.GetRequiredService<MainForm>());
        }
    }
}
