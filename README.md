中文|[English](https://github.com/xuzeyu91/AntSK/blob/main/README.en.md)
# AntSK
## 基于.Net8+AntBlazor+SemanticKernel 打造的AI知识库/智能体

## 核心功能

- **语义内核 (Semantic Kernel)**：采用领先的自然语言处理技术，准确理解、处理和响应复杂的语义查询，为用户提供精确的信息检索和推荐服务。

- **内存内核 (Kernel Memory)**：具备持续学习和存储知识点的能力，AntSK 拥有长期记忆功能，累积经验，提供更个性化的交互体验。

- **知识库**：通过文档（Word、PDF、Excel、Txt、Markdown、Json、PPT）等形式导入知识库，可以进行知识库文档。

- **API插件系统**：开放式API插件系统，允许第三方开发者或服务商轻松将其服务集成到AntSK，不断增强应用功能。

- **联网搜索**：AntSK，实时获取最新信息，确保用户接受到的资料总是最及时、最相关的。

- **GPTs 生成**：此平台支持创建个性化的GPT模型，尝试构建您自己的GPT模型。

- **API接口发布**：将内部功能以API的形式对外提供，便于开发者将AntSK 集成进其他应用，增强应用智慧。

- **模型管理**：适配和管理集成不同厂商的不同模型。

## 应用场景

AntSK 适用于多种业务场景，例如：
- 企业级知识管理系统
- 自动客服与聊天机器人
- 企业级搜索引擎
- 个性化推荐系统
- 智能辅助写作
- 教育与在线学习平台
- 其他有意思的AI App

## 功能示例

[视频示例](https://www.bilibili.com/video/BV1zH4y1h7Y9/)

首先需要创建知识库
![知识库](https://github.com/xuzeyu91/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93.png)

在知识库里可以使用文档或者url进行导入
![知识库详情](https://github.com/xuzeyu91/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93%E8%AF%A6%E6%83%85.png)

点击查看可以查看知识库的文档切片情况
![文档切片](https://github.com/xuzeyu91/AntSK/blob/main/images/%E6%96%87%E6%A1%A3%E5%88%87%E7%89%87.png)

然后我们需要创建应用，可以创建对话应用和知识库。
![应用](https://github.com/xuzeyu91/AntSK/blob/main/images/%E5%BA%94%E7%94%A8.png)

知识库应用需要选择已有的知识库，可以选多个
![应用配置](https://github.com/xuzeyu91/AntSK/blob/main/images/%E5%BA%94%E7%94%A8%E9%85%8D%E7%BD%AE.png)

然后再对话中可以对知识库的文档进行提问
![问答](https://github.com/xuzeyu91/AntSK/blob/main/images/%E9%97%AE%E7%AD%94.png)

另外我们也可以创建对话应用，可以在对应应用中配置提示词模板
![对话应用](https://github.com/xuzeyu91/AntSK/blob/main/images/%E7%AE%80%E5%8D%95%E5%AF%B9%E8%AF%9D.png)

下面来看看效果吧
![对话效果](https://github.com/xuzeyu91/AntSK/blob/main/images/%E5%AF%B9%E8%AF%9D%E6%95%88%E6%9E%9C.png)

## 如何开始？

在这里我使用的是Postgres 作为数据存储和向量存储，因为Semantic Kernel和Kernel Memory都支持他，当然你也可以换成其他的。

模型默认支持openai,如果需要使用azure openai需要调整SK的依赖注入，也可以使用one-api进行集成。

Login是默认的登陆账号和密码

需要配置如下的配置文件

## 使用docker-compose 

从项目根目录下载docker-compose.yml,然后把配置文件appsettings.json和它放在统一目录，

这里已经把pg的镜像做好了。在docker-compose.yml中可以修改默认账号密码，然后你的appsettings.json的数据库连接需要保持一致。

然后你可以进入到目录后执行
```
docker-compose up -d
```
来启动AntSK


## 配置文件的一些含义
```
  "ConnectionStrings": {
    "Postgres": "Host=;Port=;Database=antsk;Username=;Password="//这个是业务数据的连接字符串
  },
  "OpenAIOption": {
    "EndPoint": "", //openai协议的接口，写到v1之前
    "Key": "",//接口秘钥，如果使用本地模型可以随意填写一个但不能为空
    "Model": "",//会话模型，使用接口时需要，使用本地模型可以随意填写
    "EmbeddingModel": ""//向量模型，使用接口时需要，使用本地模型可以随意填写
  },
  "Postgres": {
    "ConnectionString": "Host=;Port=;Database=antsk;Username=;Password=",
    "TableNamePrefix": "km-"
  },
  "Login": {
    "User": "admin",
    "Password": "xuzeyu"
  }
```
我使用的是CodeFirst模式，只要配置好数据库链接，表结构是自动创建的

如果想使用LLamaSharp运行本地模型还需要设置如下配置：
```
  "LLamaSharp": {
    "Chat": "D:\\Code\\AI\\AntBlazor\\model\\tinyllama-1.1b-chat.gguf",//本地会话模型的磁盘路径
    "Embedding": "D:\\Code\\AI\\AntBlazor\\model\\tinyllama-1.1b-chat.gguf"//本地向量模型的磁盘路径
  },
```

需要配置Chat和Embedding模型的地址，然后修改EndPoint为本地，使用本地模型时并没有用到Key、Model、EmbeddingModel这些参数，所以这几个你可以随意填写：
```
 "OpenAIOption": {
    "EndPoint": "http://ip:port/llama/",//如果使用本地模型这个ip端口是AntSK服务启动的ip和端口
    "Key": "",//接口秘钥，如果使用本地模型可以随意填写一个但不能为空
    "Model": "",//会话模型，使用接口时需要，使用本地模型可以随意填写
    "EmbeddingModel": ""//向量模型，使用接口时需要，使用本地模型可以随意填写
  },
```


想了解更多信息或开始使用 **AntSK**，可以关注我的公众号以及加入交流群。

## 联系我
如有任何问题或建议，请通过以下方式关注我的公众号，发消息与我联系，我们也有交流群，可以发送进群等消息，然后我会拉你进交流群
![公众号](https://github.com/xuzeyu91/Avalonia-Assistant/blob/main/img/gzh.jpg)

---

我们对您在**AntSK**的兴趣表示感谢，并期待与您携手共创智能化的未来！
