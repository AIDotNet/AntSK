using AntSK.LLamaFactory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntSK.LLamaFactory
{
    public class LLamaFactoryService
    {
        List<LLamaFactoryModel> Models = new List<LLamaFactoryModel>();
        public LLamaFactoryService() {
            
        }

        public List<LLamaFactoryModel> GetLLamaFactoryModels()
        {
            if (Models == null)
            {
                string jsonString = File.ReadAllText("modelList.json");

                // 反序列化 JSON 字符串到相应的 C# 对象
                var myData = JsonSerializer.Deserialize<List<LLamaFactoryModel>>(jsonString);
            }
            return Models;
        }
    }
}
