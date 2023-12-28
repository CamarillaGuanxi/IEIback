using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
namespace IeIAPI
{
    public class Program
    {
      
            public static void Main(string[] args)
            {
                CreateWebHostBuilder(args).Build().Run();
            }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseUrls("http://0.0.0.0:5000");
    }
 
}
