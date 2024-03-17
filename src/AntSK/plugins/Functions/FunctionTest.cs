using AntSK.Domain.Common;
using AntSK.Domain.Repositories;
using System.ComponentModel;

namespace AntSK.plugins.Functions
{
    public class FunctionTest(IAIModels_Repositories Repository)
    {
        [Description("AntSK:获取订单信息")]
        [return: Description("订单信息")]
        public string GetOrder([Description("订单号")]  string id)
        {
            return $"""
                    订单ID: {id}
                    商品名：小米MIX4
                    数量：1个
                    价格：4999元
                    收货地址：上海市黄浦区
                """;
        }



        [Description("AntSK:获取模型")]
        [return: Description("模型列表")]
        public string GetModels()
        {
            var models = Repository.GetList();
            return string.Join(",", models.Select(x => x.ModelName));
        }
    }
}