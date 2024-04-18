using System.ComponentModel;

namespace AntSK.Test
{
    /// <summary>
    /// 测试插件导入
    /// </summary>
    public class TestFunctionImport
    {
        //导入的Function Call插件请安此格式书写，函数描述需要以AntSK:开头,返回值描述需要以return:Description,参数描述需要使用Description
        [Description("AntSK:获取产品详情")]
        [return: Description("返回信息详情")]
        public string GetInfo([Description("产品名称")] string Name)
        {
            return $"""
                   我的名字是{Name},
                   我的作者是许泽宇
                   我是一个AI 知识库/智能体项目
                """;
        }
    }
}
