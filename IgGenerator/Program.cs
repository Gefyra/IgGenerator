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
                .AddSingleton<Application>(new Application())
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}