using IgGenerator.DataObjectHandling;
using Microsoft.Extensions.DependencyInjection;

namespace IgGenerator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ServiceProvider services = CreateServices();

            Application app = services.GetRequiredService<Application>();
            app.StartWorkflow();
        }

        private static ServiceProvider CreateServices()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(new Application())
                .AddSingleton<IDataObjectTemplateHandler, DataObjectTemplateHandler>()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}