using System.Threading;

namespace AntSK.BackgroundTask
{
	public interface IBackgroundTaskBroker
	{
		void Start(int workerCount, CancellationToken cancellationToken);

		void Stop();
	}
}
