using Domain;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.HostedServices
{
	public class ScheduleNotifierStarter : BackgroundService
	{
		static private readonly TimeSpan UpdateTimer = new(0, 0, 40);

		private readonly ScheduleNotifier _scheduleNotifier;

		public ScheduleNotifierStarter(ScheduleNotifier scheduleNotifier)
		{
			_scheduleNotifier = scheduleNotifier ?? throw new ArgumentNullException(nameof(scheduleNotifier));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			for (; !stoppingToken.IsCancellationRequested;)
			{
				_scheduleNotifier.Notify();
				await Task.Delay(UpdateTimer, stoppingToken);
			}
		}
	}
}
