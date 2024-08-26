using Newtonsoft.Json;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Python.Runtime.Py;
using AntSK.Domain.Utils;

namespace AntSK.Domain.Domain.Other.Bge
{
    public static class BegRerankConfig
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
                    PyRunTime.InitRunTime(pythondllPath);
                    try
                    {
                        using (GIL())// 初始化Python环境的Global Interpreter Lock)
                        {
                            dynamic modelscope = Py.Import("modelscope");
                            dynamic flagEmbedding = Py.Import("FlagEmbedding");

                            dynamic model_dir = modelscope.snapshot_download(modelName, revision: "master");
                            dynamic flagReranker = flagEmbedding.FlagReranker(model_dir, use_fp16: false);
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


        public static double Rerank(List<string> list)
        {
            using (GIL())
            {
                try
                {
                    PyList pyList = new PyList();
                    foreach (string item in list)
                    {
                        pyList.Append(item.ToPython()); // 将C# string转换为Python对象并添加到PyList中
                    }
                    PyObject result = model.compute_score(pyList, normalize: true);
                    return result.ConvertToString().Trim('[').Trim(']').ConvertToDouble();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
