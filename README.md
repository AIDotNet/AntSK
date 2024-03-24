中文|[English](https://github.com/AIDotNet/AntSK/blob/main/README.en.md)
# AntSK
## 基于.Net8+AntBlazor+SemanticKernel 打造的AI知识库/智能体

## 核心功能

- **语义内核 (Semantic Kernel)**：采用领先的自然语言处理技术，准确理解、处理和响应复杂的语义查询，为用户提供精确的信息检索和推荐服务。

- **内存内核 (Kernel Memory)**：具备持续学习和存储知识点的能力，AntSK 拥有长期记忆功能，累积经验，提供更个性化的交互体验。

- **知识库**：通过文档（Word、PDF、Excel、Txt、Markdown、Json、PPT）等形式导入知识库，可以进行知识库问答。

- **GPTs 生成**：此平台支持创建个性化的GPT模型，尝试构建您自己的GPT模型。

- **API接口发布**：将内部功能以API的形式对外提供，便于开发者将AntSK 集成进其他应用，增强应用智慧。

- **API插件系统**：开放式API插件系统，允许第三方开发者或服务商轻松将其服务集成到AntSK，不断增强应用功能。

- **.Net插件系统**：开放式dll插件系统，允许第三方开发者或服务商轻松将其业务功能通过标准格式的代码生成dll后集成到AntSK，不断增强应用功能。

- **联网搜索**：AntSK，实时获取最新信息，确保用户接受到的资料总是最及时、最相关的。

- **模型管理**：适配和管理集成不同厂商的不同模型。并且支持**llama.cpp**所支持的gguf类型，以及**llamafactory**所支持的模型离线运行

- **国产信创**：AntSK支持国产模型，和国产数据库，可以在信创条件下运行

- **模型微调**：规划中，基于llamafactory进行模型微调
  

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


### 其他功能示例
[视频示例](https://www.bilibili.com/video/BV1zH4y1h7Y9/)

首先需要创建知识库
![知识库](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93.png)

在知识库里可以使用文档或者url进行导入
![知识库详情](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93%E8%AF%A6%E6%83%85.png)

点击查看可以查看知识库的文档切片情况
![文档切片](https://github.com/AIDotNet/AntSK/blob/main/images/%E6%96%87%E6%A1%A3%E5%88%87%E7%89%87.png)

然后我们需要创建应用，可以创建对话应用和知识库。
![应用](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%BA%94%E7%94%A8.png)

知识库应用需要选择已有的知识库，可以选多个
![应用配置](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%BA%94%E7%94%A8%E9%85%8D%E7%BD%AE.png)

然后再对话中可以对知识库的文档进行提问
![问答](https://github.com/AIDotNet/AntSK/blob/main/images/%E9%97%AE%E7%AD%94.png)

另外我们也可以创建对话应用，可以在对应应用中配置提示词模板
![对话应用](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%AE%80%E5%8D%95%E5%AF%B9%E8%AF%9D.png)

下面来看看效果吧
![对话效果](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%AF%B9%E8%AF%9D%E6%95%88%E6%9E%9C.png)

## 如何开始？

在这里我使用的是Postgres 作为数据存储和向量存储，因为Semantic Kernel和Kernel Memory都支持他，当然你也可以换成其他的。

模型默认支持openai、azure openai 和llama支持的gguf本地模型,如果需要使用其他模型，可以使用one-api进行集成。

配置文件中的Login配置是默认的登陆账号和密码

需要配置如下的配置文件

## 使用docker-compose 

提供了pg版本 **appsettings.json** 和 简化版本（Sqlite+disk） **docker-compose.simple.yml**

从项目根目录下载**docker-compose.yml**,然后把配置文件**appsettings.json**和它放在统一目录，

这里已经把pg的镜像做好了。在docker-compose.yml中可以修改默认账号密码，然后你的**appsettings.json**的数据库连接需要保持一致。

然后你可以进入到目录后执行
```
docker-compose up -d
```
来启动AntSK

## 如何在docker中挂载本地模型，和模型下载的目录
```
# 非 host 版本, 不使用本机代理
version: '3.8'
services:
  antsk:
    container_name: antsk
    image: registry.cn-hangzhou.aliyuncs.com/AIDotNet/antsk:v0.1.5
    ports:
      - 5000:5000
    networks:
      - antsk
    depends_on:
      - antskpg
    restart: always
    environment:
      - ASPNETCORE_URLS=http://*:5000
    volumes:
      - ./appsettings.json:/app/appsettings.json # 本地配置文件 需要放在同级目录
      - D://model:/app/model
networks:
  antsk:
```
以这个为示例，意思是把windows本地D://model的文件夹挂载进 容器内/app/model 如果是这样你的appsettings.json中的模型地址应该配置为  
```
model/xxx.gguf
```

## 配置文件的一些含义
```
{
  "DBConnection": {
    "DbType": "Sqlite", 
    "ConnectionStrings": "Data Source=AntSK.db;"
  },
  "KernelMemory": {
    "VectorDb": "Disk", 
    "ConnectionString": "Host=;Port=;Database=antsk;Username=;Password=",
    "TableNamePrefix": "km-"
  },
  "LLamaSharp": {
    "RunType": "GPU", 
    "Chat": "D:\\Code\\AI\\AntBlazor\\model\\qwen1_5-1_8b-chat-q8_0.gguf",
    "Embedding": "D:\\Code\\AI\\AntBlazor\\model\\qwen1_5-1_8b-chat-q8_0.gguf",
    "FileDirectory": "D:\\Code\\AI\\AntBlazor\\model\\"
  },
  "Login": {
    "User": "admin",
    "Password": "xuzeyu"
  },
  "BackgroundTaskBroker": {
    "ImportKMSTask": {
      "WorkerCount": 1 
    }
  }
}
```
```
//支持多种数据库，具体可以查看SqlSugar，MySql，SqlServer，Sqlite，Oracle，PostgreSQL，Dm，Kdbndp，Oscar，MySqlConnector，Access，OpenGauss，QuestDB，HG，ClickHouse，GBase，Odbc，OceanBaseForOracle，TDengine，GaussDB，OceanBase，Tidb，Vastbase，PolarDB，Custom
DBConnection.DbType
//连接字符串，需要根据不同DB类型，用对应的字符串
DBConnection.ConnectionStrings

//向量存储的类型，支持  Postgres  Disk  Memory ，其中Postgres需要配置 ConnectionString
KernelMemory.VectorDb

//本地模型使用的运行方式  GUP  CPU ,如果用在线API 这个随意使用一个即可
LLamaSharp.RunType
//本地会话模型的模型路径 注意区分linux和windows盘符不同
LLamaSharp.Chat
//本地向量模型的模型路径 注意区分linux和windows盘符不同
LLamaSharp.Embedding
//本地模型路径，用于在选择llama时可以快速选择目录下的模型，以及保存下载的模型
LLamaSharp.FileDirectory

//默认管理员账号密码
Login
//导入异步处理的线程数，使用在线API可以高一点，本地模型建议1 否则容易内存溢出崩掉
BackgroundTaskBroker.ImportKMSTask.WorkerCount
```

## 找不到样式问题解决：
AntSK/src/AntSK下执行:
```
dotnet clean
dotnet build
dotnet publish "AntSK.csproj"
```
再去AntSK/src/AntSK/bin/Release/net8.0/publish下
```
dotnet AntSK.dll
```
然后启动就有样式了

DB我使用的是CodeFirst模式，只要配置好数据库链接，表结构是自动创建的

## 使用llamafactory
```
1、首先需要确保你的环境已经安装了python和pip，如果使用镜像，例如v0.2.3.2版本已经包含了 python全套环境则无需此步骤
2、进入模型添加页面选择llamafactory
3、点击初始化，可以检查pip install 环境是否完成
4、选择一个喜欢的模型
5、点击启动,这会开始从魔塔下载模型，你可能需要有一个较为漫长的等待
6、等待模型下载完毕后，在请求地址输入 http://localhost:8000/   这里默认是使用8000端口
7、点击保存，然后就可以开始聊天了
8、很多人会问 LLamaSharp与llamafactory有什么区别？其实这两者LLamaSharp是llama.cpp的 dotnet实现，但是只支持本地gguf模型，  而llamafactory 支持的模型种类更多，但使用的是python的实现，其主要差异在这里，另外llamafactory具有模型微调的能力，这也是我们下一步需要重点集成的部分。
```

想了解更多信息或开始使用 **AntSK**，可以关注我的公众号以及加入交流群。

## 联系我
如有任何问题或建议，请通过以下方式关注我的公众号，发消息与我联系，我们也有交流群，可以发送进群等消息，然后我会拉你进交流群
![公众号](https://github.com/AIDotNet/Avalonia-Assistant/blob/main/img/gzh.jpg)

---

我们对您在**AntSK**的兴趣表示感谢，并期待与您携手共创智能化的未来！

