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
using Python.Runtime;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using AntSK.Domain.Repositories;
using DocumentFormat.OpenXml.EMMA;

namespace AntSK.Domain.Domain.Service
{
    [ServiceDescription(typeof(IPyNetEmbeddingService), ServiceLifetime.Singleton)]
    public class PyNetEmbeddingService : IPyNetEmbeddingService
    {
        private readonly IAIModels_Repositories _aIModels_Repositories;
        public PyNetEmbeddingService(IAIModels_Repositories aIModels_Repositories)
        {
            _aIModels_Repositories = aIModels_Repositories;
        }

        public static dynamic model { get; set; }
        /// <summary>
        /// 模型写死
        /// </summary>
        public dynamic LoadModel()
        {
            if (model == null)
            {
                Runtime.PythonDLL = @"D:\Programs\Python\Python311\python311.dll";
                PythonEngine.Initialize();
                Py.GIL();// 初始化Python环境的Global Interpreter Lock
                try
                {
                    dynamic modelscope = Py.Import("modelscope");
                    dynamic model_dir = modelscope.snapshot_download("AI-ModelScope/bge-large-zh-v1.5", revision: "master");
                    dynamic HuggingFaceBgeEmbeddingstemp = Py.Import("langchain.embeddings");
                    dynamic HuggingFaceBgeEmbeddings = HuggingFaceBgeEmbeddingstemp.HuggingFaceBgeEmbeddings;
                    string model_name = model_dir;
                    dynamic model_kwargs = new PyDict();
                    model_kwargs["device"] = new PyString("cpu");
                    dynamic model = HuggingFaceBgeEmbeddings(
              model_name: model_dir,
              model_kwargs: model_kwargs
          );
                    return model;
                }
                catch
                {
                    return null;
                }

            }
            else
                return model;
        }

        public string GetEmbedding(string queryStr)
        {
            var queryResult = model.embed_query(queryStr).ToString();
            return queryResult;
        }
    }
}
