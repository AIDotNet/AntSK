using AntSK.Domain.Common;

namespace AntSK.plugins.Functions
{
    public class FunctionTest
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns>订单信息</returns>
        [AntSkFunction]
        public string GetOrder(int id)
        {
            return $"""
                    订单ID: {id}
                    商品名：小米MIX4
                    数量：1个
                    价格：4999元
                    收货地址：上海市黄浦区
                """;
        }
    }
}