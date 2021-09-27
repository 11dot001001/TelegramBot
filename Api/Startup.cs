using Api.Clients;
using Api.HostedServices;
using Data;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Api
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
            services.AddRouting();
            services.AddCors(c => c.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            }));

            services.AddSingleton<ITelegramBotClient, ConfiguredTelegramBotClient>();
            services.AddHostedService<TelegramMessageReceiver>();
            services.AddHostedService<ScheduleNotifierStarter>();

            services.AddData(); 
            services.AddDomain();

            services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Документация API", Version = "v1" });
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors();
			app.UseRouting();

            app.UseEndpoints(endpointRouteBuilder =>
			{
				endpointRouteBuilder.MapControllers();
			});

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "default");
            });

            // Добавляем роут которые редиректнет пользователя в свагер при пустом роуте.
            app.Map(string.Empty, appBuilder =>
            {
                appBuilder.Run(async context => await Task.Run(() =>
                {
                    var options = app.ApplicationServices.GetService<IOptions<SwaggerUIOptions>>().Value;
                    context.Response.Redirect(options.RoutePrefix);
                }));
            });
        }
	}
}
