using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.HostedServices
{
	public class ScheduleNotifierStarter : BackgroundService
	{
		static private readonly TimeSpan UpdateTimer = new(0, 0, 40);

		private readonly ScheduleNotifier _scheduleNotifier;
		private readonly ILogger<ScheduleNotifierStarter> _logger;

		public ScheduleNotifierStarter(ScheduleNotifier scheduleNotifier, ILogger<ScheduleNotifierStarter> logger)
		{
			_scheduleNotifier = scheduleNotifier ?? throw new ArgumentNullException(nameof(scheduleNotifier));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			for (; !stoppingToken.IsCancellationRequested;)
			{
				_logger.LogInformation($"Start schedule notifications.");
				try
				{
					_scheduleNotifier.Notify();
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
				}
				_logger.LogInformation($"End schedule notifications.");
				await Task.Delay(UpdateTimer, stoppingToken);
			}
			_logger.LogInformation($"Stop execution {nameof(ScheduleNotifierStarter)}.");
		}
	}
}
