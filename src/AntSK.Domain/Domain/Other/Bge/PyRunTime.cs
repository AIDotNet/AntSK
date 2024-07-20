using Python.Runtime;

namespace AntSK.Domain.Domain.Other.Bge
{
    public static class PyRunTime
    {
        static object lockobj = new object();

        static bool isInit = false;

        public static void InitRunTime(string pythonPath)
        {
            lock (lockobj)
            {
                if (!isInit)
                {
                    if (string.IsNullOrEmpty(Runtime.PythonDLL))
                    {
                        Runtime.PythonDLL = pythonPath;
                    }
                    PythonEngine.Initialize();
                    PythonEngine.BeginAllowThreads();
                    isInit = true;
                }
            }
        }
    }
}
