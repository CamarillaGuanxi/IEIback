using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IeIAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
                   webBuilder.UseUrls("https://ieiapi.onrender.com/api/datos");  // Cambia aquí el puerto y la dirección
               });
    }
}
