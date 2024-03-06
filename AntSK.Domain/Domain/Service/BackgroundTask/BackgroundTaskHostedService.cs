using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AntSK.Domain.BackgroundTask
{
	public class BackgroundTaskHostedService : BackgroundService
	{
		private readonly ILogger<BackgroundTaskHostedService> _logger;

		private readonly BackgroundTaskBrokerOptions _options;

		private readonly IEnumerable<IBackgroundTaskBroker> _brokers;

		public BackgroundTaskHostedService(ILogger<BackgroundTaskHostedService> logger, IEnumerable<IBackgroundTaskBroker> brokers, IOptions<BackgroundTaskBrokerOptions> options)
		{
			_logger = logger;
			_brokers = brokers;
			_options = options.Value;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			foreach (IBackgroundTaskBroker broker in _brokers)
			{
				Type type = broker.GetType();
				BackgroundTaskBrokerConfig backgroundTaskBrokerConfig = _options.BrokerConfigs[type];
				if (backgroundTaskBrokerConfig == null)
				{
					throw new ArgumentNullException("config");
				}
				broker.Start(backgroundTaskBrokerConfig.WorkerCount, stoppingToken);
			}
			return Task.CompletedTask;
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("BackgroundTask Hosted Service is stopping.");
			foreach (IBackgroundTaskBroker item in _brokers)
			{
				item.Stop();
			}
			await base.StopAsync(stoppingToken);
		}
	}
}
