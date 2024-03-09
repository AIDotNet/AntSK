using Microsoft.Extensions.DependencyInjection;

namespace AntSK.BackgroundTask
{
    internal class DefaultBackgroundTaskBulider : IBackgroundTaskBulider
    {
        public IServiceCollection Services { get; }

        public DefaultBackgroundTaskBulider(IServiceCollection services)
        {
            Services = services;
        }
    }
}
