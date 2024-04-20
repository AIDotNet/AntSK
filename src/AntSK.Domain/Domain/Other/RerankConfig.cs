using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Other
{
    public static class RerankConfig
    {
        public static dynamic model { get; set; }

        static object lockobj = new object();



        /// <summary>
        /// 模型写死
        /// </summary>
        public static dynamic LoadModel(string pythondllPath, string modelName)
        {
            lock (lockobj)
            {
                if (model == null)
                {
                    Runtime.PythonDLL = pythondllPath;
                    PythonEngine.Initialize();
                    PythonEngine.BeginAllowThreads();
                    try
                    {
                        using (Py.GIL())// 初始化Python环境的Global Interpreter Lock)
                        {
                            dynamic modelscope = Py.Import("modelscope");
                            dynamic flagEmbedding = Py.Import("FlagEmbedding");

                            dynamic model_dir = modelscope.snapshot_download(modelName, revision: "master");
                            dynamic flagReranker = flagEmbedding.FlagReranker(model_dir, use_fp16: true);
                            model = flagReranker;
                            return model;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    return model;
                }
            }
        }
    }
}
