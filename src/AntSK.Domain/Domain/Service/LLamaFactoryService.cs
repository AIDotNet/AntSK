using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(ILLamaFactoryService), ServiceLifetime.Singleton)]
    public class LLamaFactoryService : ILLamaFactoryService
    {
        private Process process;

        public static bool isProcessComplete = false;

        private readonly object _syncLock = new object();

        public LLamaFactoryService() { }

        public async Task<bool> StartProcess(string modelName, string templateName)
        {
            var cmdTask = Task.Factory.StartNew(() =>
            {

                var isProcessComplete = false;

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = "api_demo.py --model_name_or_path " + modelName + " --template " + templateName + " ",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        WorkingDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "llamafactory"),
                    }
                };
                process.StartInfo.Environment["CUDA_VISIBLE_DEVICES"] = "0";
                process.StartInfo.Environment["API_PORT"] = "8000";
                process.StartInfo.EnvironmentVariables["USE_MODELSCOPE_HUB"] = "1";

                using (Process start = Process.Start(process.StartInfo))
                {
                    using (StreamReader reader = start.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        if (result != null)
                        {
                            if (result.Contains(":8000"))
                            {
                                isProcessComplete = true;
                            }
                        }
                        Console.WriteLine(result);
                    }
                    start.WaitForExit();
                }

            }, TaskCreationOptions.LongRunning);
            return true;
        }

        public string WaitForProcessExit()
        {
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        public void KillProcess()
        {

            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process1 in processes)
                {
                    if (process1.ProcessName.ToLower() == "python")
                    {
                        process1.Kill();
                        System.Console.WriteLine("kill python");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // Process already exited.
            }

        }
    }
}
