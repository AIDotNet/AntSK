中文|[English](./README.md) 
# AntSK
## 使用.Net8+Blazor+SemanticKernel 打造的AI知识库/智能体

## ⭐核心功能

- **语义内核 (Semantic Kernel)**：采用领先的自然语言处理技术，准确理解、处理和响应复杂的语义查询，为用户提供精确的信息检索和推荐服务。

- **内存内核 (Kernel Memory)**：具备持续学习和存储知识点的能力，AntSK 拥有长期记忆功能，累积经验，提供更个性化的交互体验。

- **知识库**：通过文档（Word、PDF、Excel、Txt、Markdown、Json、PPT）等形式导入知识库，可以进行知识库问答，支持本地bge-embedding 向量模型 ，以及bge-rerank 重排模型。

- **文生图**：集成**StableDiffusion** 本地模型，可以进行文生图。

- **GPTs 生成**：此平台支持创建个性化的GPT模型，尝试构建您自己的GPT模型。

- **API接口发布**：将内部功能以API的形式对外提供，便于开发者将AntSK 集成进其他应用，增强应用智慧。

- **API插件系统**：开放式API插件系统，允许第三方开发者或服务商轻松将其服务集成到AntSK，不断增强应用功能。

- **.Net插件系统**：开放式dll插件系统，允许第三方开发者或服务商轻松将其业务功能通过标准格式的代码生成dll后集成到AntSK，不断增强应用功能。

- **联网搜索**：AntSK，实时获取最新信息，确保用户接受到的资料总是最及时、最相关的。

- **模型管理**：适配和管理集成不同厂商的不同模型。并且支持**llama.cpp**所支持的gguf类型，以及**llamafactory** 和 **ollama** 所支持的模型离线运行

- **国产信创**：AntSK支持国产模型，和国产数据库，可以在信创条件下运行

- **模型微调**：规划中，基于llamafactory进行模型微调
  

## ⛪应用场景

AntSK 适用于多种业务场景，例如：
- 企业级知识管理系统
- 自动客服与聊天机器人
- 企业级搜索引擎
- 个性化推荐系统
- 智能辅助写作
- 教育与在线学习平台
- 其他有意思的AI App

## ✏️功能示例
### 在线演示

[体验地址1](https://demo.antsk.cn/)

和

[体验地址2](https://antsk.ai-dotnet.com/)
```
默认账号：test

默认密码：test

由于云服务器配置较低，无法运行本地模型，所以把系统设置权限关闭了，大家看看界面即可，要使用本地模型，请下载自行使用

请勿在演示站点上传敏感信息
```

### 其他功能示例
[视频示例](https://www.bilibili.com/video/BV1zH4y1h7Y9/)

[在线文档：http://antsk.cn](http://antsk.cn)

## ❓如何开始？

在这里我使用的是Postgres 作为数据存储和向量存储，因为Semantic Kernel和Kernel Memory都支持他，当然你也可以换成其他的。

模型默认支持openai、azure openai、讯飞星火、阿里云积、 和llama支持的gguf本地模型 以及llamafactory的本地模型,如果需要使用其他模型，可以使用one-api进行集成。

配置文件中的Login配置是默认的登录账号和密码

需要配置如下的配置文件

## 1️⃣使用docker-compose 

提供了pg版本 **appsettings.json** 和 简化版本（**Sqlite+disk**） **docker-compose.simple.yml**

从项目根目录下载**docker-compose.yml**,然后把配置文件**appsettings.json**和它放在统一目录，

这里已经把pg的镜像做好了。在docker-compose.yml中可以修改默认账号密码，然后你的**appsettings.json**的数据库连接需要保持一致。

然后你可以进入到目录后执行
```
docker-compose up -d
```
来启动AntSK

## 2️⃣如何在docker中挂载本地模型，和模型下载的目录
```
# 非 host 版本, 不使用本机代理
version: '3.8'
services:
  antsk:
    container_name: antsk
    image: registry.cn-hangzhou.aliyuncs.com/AIDotNet/antsk:v0.3.1
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
      - D://model:/root/.cache/modelscope/hub/AI-ModelScope #使用Llamafactory时需要挂载 否则初始化的环境重启后会丢失
networks:
  antsk:
```
以这个为示例，意思是把windows本地D://model的文件夹挂载进 容器内/app/model 如果是这样你的appsettings.json中的模型地址应该配置为  

[LiteDockerCompose](https://github.com/AIDotNet/AntSK/blob/main/docker-compose.simple.yml)

精简版使用sqlite+disk向量模式，简化部署配置

[FullDockerCompose](https://github.com/AIDotNet/AntSK/blob/main/docker-compose.yml)

完整版使用pg+aspire 功能更完整，配置文件需要参考如下配置含义进行配置


## 3️⃣配置文件的一些含义
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
  "FileDir": {
    "DirectoryPath": "D:\\git\\AntBlazor\\model"
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

//向量存储的类型，支持  Postgres、Disk、Memory、Qdrant、Redis、AzureAISearch
//Postgres、Redis需要配置 ConnectionString
//Qdrant 和AzureAISearch 的 ConnectionString 使用 Endpoint|APIKey  
KernelMemory.VectorDb

//本地模型路径，用于在选择llama时可以快速选择目录下的模型，以及保存下载的模型
FileDir.DirectoryPath

//默认管理员账号密码
Login
//导入异步处理的线程数，使用在线API可以高一点，本地模型建议1 否则容易内存溢出崩掉
BackgroundTaskBroker.ImportKMSTask.WorkerCount
```

## ⚠️找不到样式问题解决：
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

## ✔️使用llamafactory
```
1、首先需要确保你的环境已经安装了python和pip，如果使用镜像，例如p0.2.4版本已经包含了 python全套环境则无需此步骤
2、进入模型添加页面选择llamafactory
3、点击初始化，可以检查pip install 环境是否完成
4、选择一个喜欢的模型
5、点击启动,这会开始从魔塔下载模型，你可能需要有一个较为漫长的等待
6、等待模型下载完毕后，在请求地址输入 http://localhost:8000/   这里默认是使用8000端口
7、点击保存，然后就可以开始聊天了
8、很多人会问 LLamaSharp与llamafactory有什么区别？其实这两者LLamaSharp是llama.cpp的 dotnet实现，但是只支持本地gguf模型，  而llamafactory 支持的模型种类更多，但使用的是python的实现，其主要差异在这里，另外llamafactory具有模型微调的能力，这也是我们下一步需要重点集成的部分。
```
﻿
## 💕 贡献者

这个项目的存在要感谢所有的贡献者。
﻿
<a href="https://github.com/AIDotNet/AntSK/graphs/contributors">
<img src="https://contrib.rocks/image?repo=AIDotNet/AntSK&max=1000&columns=15&anon=1" />
</a>
﻿
## 🚨 使用协议

本仓库遵循  [AntSK License](https://github.com/AIDotNet/AntSK?tab=Apache-2.0-1-ov-file) 开源协议。

除以下附加条款外，该项目遵循Apache 2.0协议

1. 本项目可以用于商业目的，但如果违反以下规定，它有权禁止您使用
   
2. 未经授权，您不允许修改AntSK的徽标和标题信息
   
3. 未经授权，您不能修改页面底部的版权信息
   
4. 如果您需要授权，可以联系微信：xuzeyu91

如果您打算在商业项目中使用AntSK，您需要确保遵守以下步骤：

1. 包含AntSK许可证的版权声明。 [AntSK License](https://github.com/AIDotNet/AntSK?tab=Apache-2.0-1-ov-file) 。

2. 如果您修改了软件源代码，您需要在源代码中明确标明这些修改。

3. 满足以上要求

## 💕 特别感谢
助力企业级AI应用开发，推荐使用 [AntBlazor](https://antblazor.com)


## ☎️联系我
如有任何问题或建议，请通过以下方式关注我的公众号《许泽宇的技术分享》，发消息与我联系，我们也有AIDotnet交流群，可以发送进群等消息，然后我会拉你进交流群

另外您也可以通过邮箱与我联系：antskpro@qq.com

![公众号](https://github.com/AIDotNet/AntSK/blob/main/images/gzh.jpg)

## 🌟 Star History
<a href="https://github.com/AIDotNet/AntSK/stargazers" target="_blank" style="display: block" align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/svg?repos=AIDotNet/AntSK&type=Date&theme=dark" />
    <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/svg?repos=AIDotNet/AntSK&type=Date" />
    <img alt="Star History Chart" src="https://api.star-history.com/svg?repos=AIDotNet/AntSK&type=Date" />
  </picture>
</a>

