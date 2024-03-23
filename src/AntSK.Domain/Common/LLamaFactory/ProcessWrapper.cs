using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Common.LLamaFactory
{
    public class ProcessWrapper
    {
        private Process process;

        public static bool isProcessComplete = false;


        public void StartProcess(string arguments, string workingDirectory)
        {
            process =  new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }
            };
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
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch (InvalidOperationException)
            {
                // Process already exited.
            }
        }
    }
}
