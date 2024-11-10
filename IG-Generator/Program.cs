using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;

namespace IgGen
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ServiceProvider services = CreateServices();

            IgGenApplication app = services.GetRequiredService<IgGenApplication>();
            app.StartWorkflow();
        }

        private static ServiceProvider CreateServices()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<IgGenApplication>(new IgGenApplication())
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}