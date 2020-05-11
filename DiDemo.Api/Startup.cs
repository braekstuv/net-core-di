using DiDemo.Common.Services;
using DiDemo.Common.Services.DemoDependencies;
using DiDemo.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiDemo.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            InstallServices(services);
            // InstallNonDisposingServices(services);
            // InstallWronglyWiredServices(services);

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DI Demo", Version = "v1" });
            });
        }

        private void InstallServices(IServiceCollection services) {
            services.AddScoped<MyCoolService>()
                .AddScoped<MyOtherCoolService>();

            services.AddSingleton<SingletonDependency>()
                .AddScoped<ScopedDependency>()
                .AddTransient<TransientDependency>();
        }

        private void InstallNonDisposingServices(IServiceCollection services) {
            services.AddScoped<MyCoolService>()
                .AddScoped<MyOtherCoolService>();

            var singleton = new SingletonDependency();
            services.AddSingleton<SingletonDependency>(singleton)
                .AddScoped<ScopedDependency>()
                .AddTransient<TransientDependency>();
        }

        private void InstallWronglyWiredServices(IServiceCollection services) {
            //This will generate an exception, as we're wiring Singleton dependencies with Scoped dependencies.
            services.AddSingleton<MyCoolService>()
                .AddSingleton<MyOtherCoolService>();

            services.AddSingleton<SingletonDependency>()
                .AddScoped<ScopedDependency>()
                .AddTransient<TransientDependency>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DI Demo V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

    }
}