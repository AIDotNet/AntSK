using System.Threading;

namespace AntSK.Domain.BackgroundTask
{
	public interface IBackgroundTaskBroker
	{
		void Start(int workerCount, CancellationToken cancellationToken);

		void Stop();
	}
}
