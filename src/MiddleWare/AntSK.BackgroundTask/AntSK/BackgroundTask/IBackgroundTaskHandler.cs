using System.Threading.Tasks;

namespace AntSK.BackgroundTask
{
	public interface IBackgroundTaskHandler<TItem>
	{
		Task ExecuteAsync(TItem item);

		Task OnSuccess();

		Task OnFailed();
	}
}
