中文|[English](./README.en.md) 
# AntSK
## 使用.Net9 + Blazor+SemanticKernel 打造的AI知识库/智能体

![GitHub stars](https://img.shields.io/github/stars/AIDotNet/AntSK?style=social)
![GitHub forks](https://img.shields.io/github/forks/AIDotNet/AntSK?style=social)
![GitHub license](https://img.shields.io/github/license/AIDotNet/AntSK)
![.NET version](https://img.shields.io/badge/.NET-9.0-blue)

AntSK 是一个基于 .NET 9 和 Blazor 技术栈构建的企业级AI知识库和智能体平台，集成了 Semantic Kernel 和 Kernel Memory，提供完整的AI应用开发解决方案。

## 📋 目录

- [⭐ 核心功能](#核心功能)
- [🏗️ 技术架构](#技术架构)
- [🔄 系统工作流程](#系统工作流程)
- [🛠️ 技术栈](#技术栈)
- [📁 项目结构](#项目结构)
- [🚀 特色功能](#特色功能)
- [⛪ 应用场景](#应用场景)
- [✏️ 功能示例](#功能示例)
- [❓ 如何开始](#如何开始)
- [🔧 开发指南](#开发指南)
- [📊 性能优化建议](#性能优化建议)
- [💕 贡献者](#贡献者)
- [🚨 使用协议](#使用协议)
- [☎️ 联系我](#联系我)

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

## 🏗️ 技术架构

```mermaid
graph TB
    subgraph "用户界面层"
        UI[Blazor前端界面]
        API[Web API接口]
    end
    
    subgraph "应用服务层"
        Chat[聊天服务]
        KMS[知识库服务]
        Plugin[插件服务]
        Model[模型管理服务]
        Auth[认证服务]
    end
    
    subgraph "领域核心层"
        SK[Semantic Kernel]
        KM[Kernel Memory]
        Embedding[向量嵌入]
        Function[函数调用]
    end
    
    subgraph "基础设施层"
        DB[(数据库)]
        Vector[(向量数据库)]
        File[文件存储]
        OCR[OCR服务]
        SD[StableDiffusion]
    end
    
    subgraph "AI模型层"
        OpenAI[OpenAI]
        Local[本地模型]
        LlamaFactory[LlamaFactory]
        Ollama[Ollama]
        Spark[讯飞星火]
    end
    
    subgraph "插件系统"
        NetPlugin[.NET插件]
        APIPlugin[API插件]
        FuncPlugin[函数插件]
    end
    
    UI --> Chat
    UI --> KMS
    UI --> Plugin
    UI --> Model
    API --> Auth
    
    Chat --> SK
    KMS --> KM
    Plugin --> Function
    
    SK --> OpenAI
    SK --> Local
    SK --> LlamaFactory
    SK --> Ollama
    SK --> Spark
    
    KM --> Vector
    KM --> Embedding
    
    Chat --> NetPlugin
    Chat --> APIPlugin
    Chat --> FuncPlugin
    
    KMS --> DB
    KMS --> File
    Model --> DB
    
    OCR --> SD
    
    style SK fill:#e1f5fe
    style KM fill:#e8f5e8
    style UI fill:#fff3e0
    style API fill:#fff3e0
```

## 🔄 系统工作流程

```mermaid
graph TD
    A[用户输入] --> B{输入类型}
    
    B -->|文档上传| C[文档解析]
    B -->|聊天对话| D[对话处理]
    B -->|API调用| E[API处理]
    
    C --> F[文档分块]
    F --> G[向量化处理]
    G --> H[存储到知识库]
    
    D --> I{是否需要知识库}
    I -->|是| J[知识库检索]
    I -->|否| K[直接调用LLM]
    
    J --> L[向量搜索]
    L --> M[相关性排序]
    M --> N[构建Prompt]
    
    K --> O[LLM推理]
    N --> O
    
    O --> P{是否需要插件}
    P -->|是| Q[插件调用]
    P -->|否| R[生成回复]
    
    Q --> S[执行函数]
    S --> T[合并结果]
    T --> R
    
    E --> U[权限验证]
    U --> V[业务逻辑]
    V --> W[返回结果]
    
    R --> X[用户界面展示]
    W --> X
    
    style A fill:#e1f5fe
    style O fill:#e8f5e8
    style H fill:#fff3e0
    style X fill:#f3e5f5
```

## 🛠️ 技术栈

### 后端技术
- **.NET 9**: 最新的 .NET 框架，提供高性能和现代化开发体验
- **Blazor Server**: 基于服务器端渲染的现代Web UI框架
- **Semantic Kernel**: 微软开源的AI编排框架
- **Kernel Memory**: 知识库和向量存储管理
- **SqlSugar**: 高性能 ORM 框架，支持多种数据库
- **AutoMapper**: 对象映射框架

### AI & ML 技术
- **OpenAI GPT**: 支持 GPT-3.5/GPT-4 系列模型
- **Azure OpenAI**: 企业级 OpenAI 服务
- **讯飞星火**: 科大讯飞大语言模型
- **阿里云积**: 阿里云大语言模型
- **LlamaFactory**: 本地模型微调和推理
- **Ollama**: 本地模型运行环境
- **Stable Diffusion**: 文生图模型
- **BGE Embedding**: 中文向量嵌入模型
- **BGE Rerank**: 重排序模型

### 存储技术
- **PostgreSQL**: 主数据库存储
- **SQLite**: 轻量级数据库支持
- **Qdrant**: 向量数据库
- **Redis**: 缓存和向量存储
- **Disk/Memory**: 本地存储方案

### 前端技术
- **Ant Design Blazor**: 企业级UI组件库
- **Chart.js**: 数据可视化
- **Prism.js**: 代码高亮

## 📁 项目结构

```
AntSK/
├── src/
│   ├── AntSK/                          # 主应用（Blazor Server）
│   │   ├── Components/                 # 自定义组件
│   │   ├── Controllers/                # Web API控制器
│   │   ├── Pages/                      # Blazor页面
│   │   │   ├── ChatPage/              # 聊天相关页面
│   │   │   ├── KmsPage/               # 知识库管理页面
│   │   │   ├── Plugin/                # 插件管理页面
│   │   │   ├── Setting/               # 系统设置页面
│   │   │   └── User/                  # 用户管理页面
│   │   ├── Services/                  # 应用服务
│   │   └── wwwroot/                   # 静态资源
│   ├── AntSK.Domain/                  # 领域层
│   │   ├── Domain/                    # 领域模型和接口
│   │   ├── Repositories/              # 数据仓储
│   │   ├── Services/                  # 领域服务
│   │   └── Common/                    # 通用组件
│   ├── AntSK.LLM/                     # LLM集成层
│   │   ├── SparkDesk/                 # 讯飞星火集成
│   │   ├── StableDiffusion/           # SD文生图集成
│   │   └── Mock/                      # 模拟服务
│   ├── AntSK.LLamaFactory/            # LlamaFactory集成
│   ├── AntSK.OCR/                     # OCR服务
│   ├── AntSK.BackgroundTask/          # 后台任务处理
│   └── AntSK.ServiceDefaults/         # 服务默认配置
├── docs/                              # 文档
└── docker-compose.yml                 # Docker部署文件
```

### 核心模块说明

| 模块 | 功能描述 |
|------|---------|
| **AntSK** | 主应用程序，包含Blazor UI和Web API |
| **AntSK.Domain** | 领域层，包含业务逻辑、数据模型和仓储接口 |
| **AntSK.LLM** | 大语言模型集成层，支持多种AI模型 |
| **AntSK.LLamaFactory** | LlamaFactory集成，支持本地模型微调和推理 |
| **AntSK.OCR** | 光学字符识别服务 |
| **AntSK.BackgroundTask** | 后台任务处理，如知识库导入 |

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

## 🚀 特色功能

### 🤖 多模型支持
- **云端模型**: OpenAI GPT、Azure OpenAI、讯飞星火、阿里云积灵等
- **本地模型**: 支持 Ollama 和Llamafactory运行离线模型
- **LlamaFactory**: 支持主流开源模型的微调和推理
- **Ollama**: 本地模型管理和运行
- **一键切换**: 支持在不同模型间无缝切换

### 📚 智能知识库
- **多格式支持**: Word、PDF、Excel、TXT、Markdown、JSON、PPT
- **向量化存储**: BGE-embedding 中文优化向量模型
- **智能检索**: BGE-rerank 重排序提升检索精度
- **实时同步**: 知识库内容实时更新和同步

### 🔌 开放插件系统
- **.NET 插件**: 支持 DLL 格式的原生插件
- **API 插件**: 通过 HTTP API 集成外部服务
- **函数插件**: 基于 Semantic Kernel 的函数调用
- **热插拔**: 插件动态加载，无需重启系统

### 🎨 文生图能力
- **Stable Diffusion**: 集成本地 SD 模型
- **多种后端**: 支持 CPU、CUDA、ROCm 等不同计算后端
- **参数调节**: 丰富的生成参数配置
- **批量生成**: 支持批量图片生成

### 🔍 OCR 文字识别
- **图片转文字**: 支持多种图片格式的文字提取
- **多语言支持**: 中英文等多语言识别
- **高精度**: 优化的 OCR 引擎，识别准确率高

## ❓如何开始？

### 🛠️ 环境要求
- **.NET 9 SDK**: [下载地址](https://dotnet.microsoft.com/zh-cn/download/dotnet/9.0)
- **Docker** (可选): 用于容器化部署
- **Python 3.8+** (可选): 使用 LlamaFactory 时需要

### 💾 数据库支持
AntSK 支持多种数据库，通过 SqlSugar ORM 实现：
- **PostgreSQL** (推荐): 同时支持关系型数据和向量存储
- **SQLite**: 轻量级，适合开发和测试
- **MySQL**: 广泛使用的开源数据库
- **SQL Server**: 微软企业级数据库
- **Oracle**: 企业级数据库解决方案

### 🔧 向量数据库选择
- **PostgreSQL**: 使用 pgvector 扩展
- **Qdrant**: 专业向量数据库
- **Redis**: 内存向量存储
- **Disk**: 本地文件存储
- **Memory**: 内存存储 (不持久化)

模型默认支持openai、azure openai、讯飞星火、阿里云积、 和llama支持的gguf本地模型 以及llamafactory的本地模型,如果需要使用其他模型，可以使用one-api进行集成。

配置文件中的Login配置是默认的登录账号和密码

需要配置如下的配置文件

## 为了方便体验，我已经把打包好的程序放进了网盘，你只需要安装.net9环境即可运行。
[.net9环境 ](https://dotnet.microsoft.com/zh-cn/download/dotnet/9.0)

[我用夸克网盘分享了「AntSK」](https://pan.quark.cn/s/63ea02e1683e)

下载文件后启动 AntSK.exe  然后会自动打开浏览器
```
账号: admin
密码: admin
```


[源码深度解读](https://deepwiki.com/AIDotNet/AntSK)


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
    image: registry.cn-hangzhou.aliyuncs.com/AIDotNet/antsk:v0.6.0
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

## 🔧 开发指南

### 本地开发环境搭建

1. **克隆项目**
```bash
git clone https://github.com/AIDotNet/AntSK.git
cd AntSK
```

2. **安装依赖**
```bash
# 确保已安装 .NET 9 SDK
dotnet restore
```

3. **配置数据库**
- 修改 `src/AntSK/appsettings.json` 中的数据库连接字符串
- 首次运行会自动创建数据库表结构 (CodeFirst 模式)

4. **启动项目**
```bash
cd src/AntSK
dotnet run
```
访问 `https://localhost:5001` 或 `http://localhost:5000`

### 插件开发

#### .NET 插件开发
```csharp
[AntSKFunction("插件描述")]
public class MyPlugin
{
    [AntSKFunction("函数描述")]
    public async Task<string> MyFunction(string input)
    {
        // 您的业务逻辑
        return "处理结果";
    }
}
```

#### API 插件开发
创建符合 OpenAPI 规范的 HTTP 接口，AntSK 会自动解析并集成。

### 自定义模型集成

1. **实现 IChatCompletion 接口**
```csharp
public class CustomChatCompletion : IChatCompletion
{
    public async Task<IReadOnlyList<ChatMessage>> GetChatMessageContentsAsync(
        ChatHistory chatHistory, 
        PromptExecutionSettings? executionSettings = null, 
        Kernel? kernel = null, 
        CancellationToken cancellationToken = default)
    {
        // 实现您的模型调用逻辑
    }
}
```

2. **注册服务**
```csharp
services.AddSingleton<IChatCompletion, CustomChatCompletion>();
```

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

## 📊 性能优化建议

### 硬件配置推荐

| 用途 | CPU | 内存 | 存储 | GPU |
|------|-----|------|------|-----|
| 开发测试 | 4核+ | 8GB+ | SSD 50GB+ | 可选 |
| 小型部署 | 8核+ | 16GB+ | SSD 100GB+ | 可选 |
| 生产环境 | 16核+ | 32GB+ | SSD 500GB+ | RTX 3080+ |
| 大规模部署 | 32核+ | 64GB+ | SSD 1TB+ | RTX 4090+ |

### 性能调优
- **数据库连接池**: 根据并发量调整连接池大小
- **向量维度**: 根据精度需求选择合适的向量维度
- **缓存策略**: 合理使用 Redis 缓存热点数据
- **模型选择**: 根据场景选择合适的模型大小
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

1. **免费商用**：用户在不修改应用名称、logo、版权信息的情况下，可以免费用于商业目的。
2. **商业授权**：如果您满足以下任意条件之一，需取得商业授权：
   1. 修改应用名称、logo、版权信息等。
   2. 为企业客户提供多租户服务，且该服务支持 10 人或以上的使用。
   3. 预装或集成到硬件设备或产品中进行捆绑销售。
   4. 政府或教育机构的大规模采购项目，特别是涉及安全、数据隐私等敏感需求时。
   
3. 如果您需要授权，可以联系微信：**13469996907**

如果您打算在商业项目中使用AntSK，您需要确保遵守以下步骤：

1. 包含AntSK许可证的版权声明。 [AntSK License](https://github.com/AIDotNet/AntSK?tab=Apache-2.0-1-ov-file) 。

2. 如果您修改了软件源代码，您需要在源代码中明确标明这些修改。

3. 满足以上要求


## ☎️联系我
如有任何问题或建议，请通过以下方式关注我的公众号《许泽宇的技术分享》，发消息与我联系，我们也有AIDotnet交流群，可以发送进群等消息，然后我会拉你进交流群

另外您也可以通过邮箱与我联系：antskpro@qq.com
