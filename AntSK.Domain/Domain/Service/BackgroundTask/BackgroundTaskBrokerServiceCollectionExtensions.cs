using System;
using AntSK.Domain.BackgroundTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class BackgroundTaskBrokerServiceCollectionExtensions
	{
		public static IBackgroundTaskBulider AddBackgroundTaskBroker(this IServiceCollection services)
		{
			services.AddHostedService<BackgroundTaskHostedService>();
			return new DefaultBackgroundTaskBulider(services);
		}

		public static IBackgroundTaskBulider AddDelegateHandler(this IBackgroundTaskBulider bulider)
		{
			IServiceCollection services = bulider.Services;
			services.TryAddSingleton((IServiceProvider p) => ActivatorUtilities.CreateInstance<DelegateBackgroundTaskBroker>(p, new object[1] { p }));
			services.AddSingleton((Func<IServiceProvider, IBackgroundTaskBroker>)((IServiceProvider p) => p.GetService<DelegateBackgroundTaskBroker>()));
			services.AddOptions<BackgroundTaskBrokerOptions>().Configure(delegate(BackgroundTaskBrokerOptions options, IConfiguration configuration)
			{
				BackgroundTaskBrokerConfig backgroundTaskBrokerConfig = new BackgroundTaskBrokerConfig();
				configuration.GetSection("BackgroundTaskBroker:DelegateHandler").Bind(backgroundTaskBrokerConfig);
				Type typeFromHandle = typeof(DelegateBackgroundTaskBroker);
				options.BrokerConfigs[typeFromHandle] = backgroundTaskBrokerConfig;
			});
			return bulider;
		}

		public static IBackgroundTaskBulider AddHandler<TItem, THanlder>(this IBackgroundTaskBulider bulider, string name) where THanlder : class, IBackgroundTaskHandler<TItem>
		{
			IServiceCollection services = bulider.Services;
			services.TryAddSingleton<IBackgroundTaskHandler<TItem>, THanlder>();
			services.TryAddSingleton((IServiceProvider p) => ActivatorUtilities.CreateInstance<BackgroundTaskBroker<TItem>>(p, new object[1] { p }));
			services.AddSingleton((Func<IServiceProvider, IBackgroundTaskBroker>)((IServiceProvider p) => p.GetService<BackgroundTaskBroker<TItem>>()));
			services.AddOptions<BackgroundTaskBrokerOptions>().Configure(delegate(BackgroundTaskBrokerOptions options, IConfiguration configuration)
			{
				BackgroundTaskBrokerConfig backgroundTaskBrokerConfig = new BackgroundTaskBrokerConfig();
				configuration.GetSection("BackgroundTaskBroker:" + name).Bind(backgroundTaskBrokerConfig);
				Type typeFromHandle = typeof(BackgroundTaskBroker<TItem>);
				options.BrokerConfigs[typeFromHandle] = backgroundTaskBrokerConfig;
			});
			return bulider;
		}
	}
}
