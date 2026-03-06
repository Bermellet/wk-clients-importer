using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using WKClientsImporter.Interfaces;
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
            services.AddSingleton<IStorageService, JsonStorageService>();
            services.AddSingleton<ITemplateBuilder, TemplateBuilderService>();
            // Importing services
            services.AddSingleton<IDataImporter, FileImporterService>();
            services.AddSingleton<IFileFormatImporter, CsvClienteImporter>();
            services.AddSingleton<IFileFormatImporter, JsonClienteImporter>();

            var provider = services.BuildServiceProvider();
            Application.Run(provider.GetRequiredService<MainForm>());
        }
    }
}
