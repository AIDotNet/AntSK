using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Model.Constant
{
    public class KmsConstantcs
    {
        public const string KmsIdTag = "kmsid";
        public const string FileIdTag = "fileid";
        public const string AppIdTag = "appid";
        public const string KmsIndex = "kms";
        public const string FileIndex = "kms";
        public const string KmsSearchNull="知识库未搜索到相关内容";

        public const string KmsPrompt = @"使用<data></data>标记的内容作为你的知识：
<data>
{{$doc}}
</data>
--------------------------
回答要求：
- 如果你不清楚答案，你需要澄清
- 避免提及你是从<data></data>获取的知识
- 保持答案与<data></data>众描述一致
- 使用Markdown语法优化回答格式。
- 如果Markdown有图片则正常显示
--------------------------

历史聊天记录:{{ConversationSummaryPlugin.SummarizeConversation $history}}
--------------------------
用户问题: {{$input}}";

        public const string KMExcelSplit = "*&antsk_excel&*";
    }
}
