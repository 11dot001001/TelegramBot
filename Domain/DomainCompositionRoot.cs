using Domain.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
	public static class DomainCompositionRoot
	{
		public static IServiceCollection AddDomain(this IServiceCollection services)
		{
			services.AddSingleton<DataProvider>();
			services.AddSingleton<RequestHandler>();
			services.AddSingleton<TelegramMessageHandler>();
			services.AddSingleton<KeyboardService>();
			services.AddSingleton<ScheduleNotifier>();

			return services;
		}
	}
}
