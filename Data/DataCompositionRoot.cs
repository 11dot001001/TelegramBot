using Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
	public static class DataCompositionRoot
	{
		public static IServiceCollection AddData(this IServiceCollection services)
		{
			return services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("local"), ServiceLifetime.Singleton);
		}
	}
}
