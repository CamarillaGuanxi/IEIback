﻿
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
                    builder => builder.WithOrigins("https://ieiapi.onrender.com/api/datos")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Configuración de la base de la ruta si es necesario
            app.UsePathBase("/myapp");

            // Configuración adicional...

          
            app.UseCors("AllowRender");

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


