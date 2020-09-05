using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using api.Services;
using api.helper;
using api.Hubs;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            //services.AddControllers();
            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            //services.AddSingleton<IPlaceInfoService, PlaceInfoService>();
            services.AddSingleton<IUserInfoService, UserInfoService>();
            services.AddSingleton<IGameInfoService, GameInfoService>();
            services.AddSingleton<IUserGameService, UserGameService>();
            services.AddSingleton<IAdminConfigService, AdminConfigService>();
            services.AddSignalR();
            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            //    {
            //        Title = "LOTO Project API",
            //        Version = "v1",
            //        Description = "Game API",
            //    });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
         
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseRouting();
            app.UseAuthentication();
            // global cors policy////
            //app.UseCors(x => x
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());
            app.UseCors("CorsPolicy");
            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();
            //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            //app.UseSwagger();
            //app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "LOTO Project Services"));
            app.UseSignalR(s => s.MapHub<EchoHub>("/echo"));
        }
    }
}
