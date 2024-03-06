using Microsoft.Extensions.DependencyInjection;

namespace AntSK.Domain.BackgroundTask
{
	public interface IBackgroundTaskBulider
	{
		IServiceCollection Services { get; }
	}
}
