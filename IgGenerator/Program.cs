using IgGenerator.ConsoleHandling;
using IgGenerator.DataObjectHandling;
using IgGenerator.IgHandling;
using IgGenerator.ResourceHandling;
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
                .AddSingleton<IUserInteractionHandler, UserInteractionHandler>()
                .AddSingleton<IIgFileHandler, IgFileHandler>()
                .AddSingleton<IIgHandler, IgHandling.IgHandler>()
                .AddSingleton<IDataObjectTemplateHandler, DataObjectTemplateHandler>()
                .AddSingleton<IResourceFileHandler, ResourceFileHandler>()
                .AddSingleton<IResourceHandler, ResourceHandler>()
                .AddSingleton<INamingManipulationHandler, NamingManipulationHandler>()
                .BuildServiceProvider();
            return serviceProvider;
        }
    }
}