using IgGenerator.ConsoleHandling;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.ConsoleHandling.Services;
using IgGenerator.DataObjectHandling;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.IgHandling;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling;
using IgGenerator.ResourceHandling.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IgGenerator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ServiceProvider services = CreateServices();

            IApplication app = services.GetRequiredService<IApplication>();
            app.StartWorkflow();
        }

        private static ServiceProvider CreateServices()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<IApplication, Application>()
                .AddSingleton<IUserInputCacheService, UserInputCacheService>()
                .AddSingleton<IUserInteractionHandler, UserInteractionHandler>()
                .AddSingleton<IIgFileHandler, IgFileHandler>()
                .AddSingleton<IIgHandler, IgHandling.IgHandler>()
                .AddSingleton<ITemplateHandler, TemplateHandler>()
                .AddSingleton<IResourceFileHandler, ResourceFileHandler>()
                .AddSingleton<IResourceHandler, ResourceHandler>()
                .AddSingleton<INamingManipulationHandler, NamingManipulationHandler>()
                .AddSingleton<ITocFileManager, TocFileManager>()
                .BuildServiceProvider();
            return serviceProvider;
        }
    }
}