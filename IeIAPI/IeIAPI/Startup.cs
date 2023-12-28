
using Microsoft.Extensions.Options;

namespace IeIAPI
{
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
          
           
            options.AddPolicy("AllowRender",
            builder => builder.AllowAnyOrigin()  // Permitir solicitudes desde cualquier origen
                      .AllowAnyMethod()
                      .AllowAnyHeader());
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configuración de la base de la ruta si es necesario


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowRender");

                // Agregar registros de depuración
                app.Use(async (context, next) =>
                {
                    Console.WriteLine($"Request: {context.Request.Path}");
                    await next.Invoke();
                });
            }

            // Configuración adicional...

           

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}


