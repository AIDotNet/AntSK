using Microsoft.Extensions.DependencyInjection;

namespace AntSK.BackgroundTask
{
    public interface IBackgroundTaskBulider
    {
        IServiceCollection Services { get; }
    }
}
