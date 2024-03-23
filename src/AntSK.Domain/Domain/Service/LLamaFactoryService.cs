using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model.Dto;
using AntSK.Domain.Options;
using AntSK.LLamaFactory.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(ILLamaFactoryService), ServiceLifetime.Singleton)]
    public class LLamaFactoryService : ILLamaFactoryService
    {
        private Process process;

        public static bool isProcessComplete = false;

        private readonly object _syncLock = new object();
        private List<LLamaModel> modelList = new List<LLamaModel>();

        public LLamaFactoryService() { }
        public delegate Task LogMessageHandler(string message);
        public event LogMessageHandler LogMessageReceived;
        protected virtual async Task OnLogMessageReceived(string message)
        {
            LogMessageReceived?.Invoke(message);
        }

        public async Task PipInstall()
        {

            var cmdTask = Task.Factory.StartNew(() =>
            {

                var isProcessComplete = false;

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "pip",
                        Arguments = "install -r requirements.txt -i https://pypi.tuna.tsinghua.edu.cn/simple",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    }
                };  
                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    Console.WriteLine($"{eventArgs.Data}");
                    OnLogMessageReceived(eventArgs.Data);
                };
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    Console.WriteLine($"{eventArgs.Data}");
                    OnLogMessageReceived(eventArgs.Data);
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }, TaskCreationOptions.LongRunning);
        }

        public async Task StartLLamaFactory(string modelName, string templateName)
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
                        RedirectStandardError=true,
                        WorkingDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "llamafactory"),
                    }
                };
                process.StartInfo.Environment["CUDA_VISIBLE_DEVICES"] = "0";
                process.StartInfo.Environment["API_PORT"] = "8000";
                process.StartInfo.EnvironmentVariables["USE_MODELSCOPE_HUB"] = "1";
                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    Console.WriteLine($"{eventArgs.Data}");
                    OnLogMessageReceived(eventArgs.Data);
                };
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    Console.WriteLine($"{eventArgs.Data}");
                    OnLogMessageReceived(eventArgs.Data);
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }, TaskCreationOptions.LongRunning);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
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

        public List<LLamaModel> GetLLamaFactoryModels()
        {
            if (modelList.Count==0)
            {
                string jsonString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modelList.json"));

                // 反序列化 JSON 字符串到相应的 C# 对象
                var Models = JsonConvert.DeserializeObject<List<LLamaFactoryModel>>(jsonString);
                foreach (var model in Models)
                {
                    foreach (var m in model.Models)
                    {
                        modelList.Add(new LLamaModel() { Name=m.Key, ModelScope=m.Value.MODELSCOPE });
                    }
                }
            }         
            return modelList;
        }
    }
}
