using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using AntSK.Domain.Utils;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IOllamaService), ServiceLifetime.Singleton)]
    public class OllamaService : IOllamaService
    {
        private Process process;
        public delegate Task LogMessageHandler(string message);
        public event LogMessageHandler LogMessageReceived;
        protected virtual async Task OnLogMessageReceived(string message)
        {
            LogMessageReceived?.Invoke(message);
        }

        public async Task OllamaPull(string modelName)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var cmdTask = Task.Factory.StartNew(() =>
            {

                var isProcessComplete = false;

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ollama",
                        Arguments = "pull " + modelName,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    }
                };
                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    Log.Information($"{eventArgs.Data.ConvertToString()}");
                    if (!eventArgs.Data.ConvertToString().Contains("The handle is invalid"))
                    {
                        OnLogMessageReceived(eventArgs.Data.ConvertToString());
                    }
                };
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    Log.Error($"{eventArgs.Data.ConvertToString()}");
                    if (!eventArgs.Data.ConvertToString().Contains("The handle is invalid"))
                    {
                        OnLogMessageReceived(eventArgs.Data.ConvertToString());
                    }
                };
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                OnLogMessageReceived("--------------------完成--------------------");
            }, TaskCreationOptions.LongRunning);
            await cmdTask;
        }
    
    }
}
