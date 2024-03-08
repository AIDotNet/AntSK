using LLama.Common;
using LLama;
using LLamaSharp.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Service
{
    public static class LLamaConfig
    {
        static object lockobj = new object();
        /// <summary>
        /// 避免模型重复加载，本地缓存
        /// </summary>
        static Dictionary<string, (LLamaWeights, ModelParams)> dicLLamaWeights = new Dictionary<string, (LLamaWeights, ModelParams)>();
        public static (LLamaWeights, ModelParams) GetLLamaConfig(string modelPath, LLamaSharpConfig config =null)
        {
            lock (lockobj)
            {
                if (dicLLamaWeights.ContainsKey(modelPath))
                {
                    return dicLLamaWeights.GetValueOrDefault(modelPath);
                }
                else
                {
                    InferenceParams infParams = new() { AntiPrompts = ["\n\n"] };
                    LLamaSharpConfig lsConfig = new(modelPath) { DefaultInferenceParams = infParams };
                    if (config!=null)
                    {
                        lsConfig = config;
                    }
                    var parameters = new ModelParams(lsConfig.ModelPath)
                    {
                        ContextSize = lsConfig?.ContextSize ?? 2048,
                        Seed = lsConfig?.Seed ?? 0,
                        GpuLayerCount = lsConfig?.GpuLayerCount ?? 20,
                        EmbeddingMode = true
                    };
                    var weights = LLamaWeights.LoadFromFile(parameters);
                    dicLLamaWeights.Add(modelPath, (weights, parameters));
                    return (weights, parameters);
                }
            }       
        }
    }
}
