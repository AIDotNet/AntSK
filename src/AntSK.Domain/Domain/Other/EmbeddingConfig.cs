using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Python.Runtime.Py;

namespace AntSK.Domain.Domain.Other
{
    public static class EmbeddingConfig
    {
        public static dynamic model { get; set; }

        static object lockobj = new object();

        private static GILState GIL { get; set; }

        /// <summary>
        /// 模型写死
        /// </summary>
        public static dynamic LoadModel(string pythondllPath, string modelName)
        {
            lock (lockobj)
            {
                if (model == null)
                {
                    //Runtime.PythonDLL = @"D:\Programs\Python\Python311\python311.dll";
                    Runtime.PythonDLL = pythondllPath;
                    PythonEngine.Initialize();
                    GIL= Py.GIL();// 初始化Python环境的Global Interpreter Lock
                    try
                    {
                        dynamic modelscope = Py.Import("modelscope");
                        //dynamic model_dir = modelscope.snapshot_download("AI-ModelScope/bge-large-zh-v1.5", revision: "master");
                        dynamic model_dir = modelscope.snapshot_download(modelName, revision: "master");
                        dynamic HuggingFaceBgeEmbeddingstemp = Py.Import("langchain.embeddings");
                        dynamic HuggingFaceBgeEmbeddings = HuggingFaceBgeEmbeddingstemp.HuggingFaceBgeEmbeddings;
                        string model_name = model_dir;
                        dynamic model_kwargs = new PyDict();
                        model_kwargs["device"] = new PyString("cpu");
                        dynamic hugginmodel = HuggingFaceBgeEmbeddings(
                  model_name: model_dir,
                  model_kwargs: model_kwargs
              );
                        model = hugginmodel;
                        return hugginmodel;
                    }
                    catch
                    {
                        return null;
                    }

                }
                else
                    return model;
            }
        }

        public static Task<float[]> GetEmbedding(string queryStr)
        {
            PyObject queryResult = model.embed_query(queryStr);
            var floatList = queryResult.As<float[]>();
            return Task.FromResult(floatList); ;
        }

        public static void Dispose()
        {
            Console.WriteLine("python dispose");
            GIL.Dispose();
        }
    }
}
