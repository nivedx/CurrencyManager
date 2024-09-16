using CurrencyManager.Initializers;
using CurrencyManager.Models.SystemModels;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime;

namespace CurrencyManager
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var initializer = new CurrencyManagerInitializer(services);
            initializer.Initialize();

            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency manager", Version = "v1", Description = "The service for Currency management." });

            });

            services.AddSingleton<ServiceConfig>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {

                    builder.WithOrigins(_configuration.GetSection("AllowedOrigins").Get<string[]>())
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Manager"));
            }
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
