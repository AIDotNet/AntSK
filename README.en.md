[简体中文](./README.md) | English
# AntSK
## AI Knowledge Base/Intelligent Agent built on .Net8+AntBlazor+SemanticKernel

## ⭐Core Features

- **Semantic Kernel**: Utilizes advanced natural language processing technology to accurately understand, process, and respond to complex semantic queries, providing users with precise information retrieval and recommendation services.

- **Kernel Memory**: Capable of continuous learning and storing knowledge points, AntSK has long-term memory function, accumulates experience, and provides a more personalized interaction experience.

- **Knowledge Base**: Import knowledge base through documents (Word, PDF, Excel, Txt, Markdown, Json, PPT) and perform knowledge base Q&A.

- **GPT Generation**: This platform supports creating personalized GPT models, enabling users to build their own GPT models.

- **API Interface Publishing**: Exposes internal functions in the form of APIs, enabling developers to integrate AntSK into other applications and enhance application intelligence.

- **API Plugin System**: Open API plugin system that allows third-party developers or service providers to easily integrate their services into AntSK, continuously enhancing application functionality.

- **.Net Plugin System**: Open dll plugin system that allows third-party developers or service providers to easily integrate their business functions by generating dll in standard format code, continuously enhancing application functionality.

- **Online Search**: AntSK, real-time access to the latest information, ensuring users receive the most timely and relevant data.

- **Model Management**: Adapts and manages integration of different models from different manufacturers, including gguf types supported by **llama.cpp** and models offline running supported by **llamafactory**.

- **Domestic Innovation**: AntSK supports domestic models and databases and can run under domestic innovation conditions.

- **Model Fine-Tuning**: Planned based on llamafactory for model fine-tuning.

## ⛪Application Scenarios

AntSK is suitable for various business scenarios, such as:
- Enterprise knowledge management system
- Automatic customer service and chatbots
- Enterprise search engine
- Personalized recommendation system
- Intelligent writing assistance
- Education and online learning platforms
- Other interesting AI Apps

## ✏️Function Examples
### Online Demo
[document](http://antsk.cn/)

[demo](https://antsk.ai-dotnet.com/)

```
Default account: test

Default password: test

Due to the low configuration of the cloud server, the local model cannot be run, so the system settings permissions have been closed. You can simply view the interface. If you want to use the local model, please download and use it on your own.
```

### Other Function Examples
[Video Demonstration](https://www.bilibili.com/video/BV1zH4y1h7Y9/)

## ❓How to get started?

Here I am using Postgres as the data and vector storage because Semantic Kernel and Kernel Memory support it, but you can also use other options.

The model by default supports the local model of openai, azure openai, and llama. If you need to use other models, you can integrate them using one-api.

The Login configuration in the configuration file is the default login account and password.

The following configuration file needs to be configured

## 1️⃣Using docker-compose

Provided the pg version **appsettings.json** and simplified version (Sqlite+disk) **docker-compose.simple.yml**

Download **docker-compose.yml** from the project root directory and place the configuration file **appsettings.json** in the same directory.

The pg image has already been prepared. You can modify the default username and password in docker-compose.yml, and then the database connection in your **appsettings.json** needs to be consistent.

Then you can execute the following command in the directory to start AntSK
```
docker-compose up -d
```

## 2️⃣How to mount local models and model download directory in docker
```
# Non-host version, do not use local proxy
version: '3.8'
services:
  antsk:
    container_name: antsk
    image: registry.cn-hangzhou.aliyuncs.com/AIDotNet/antsk:v0.1.5ports:
      - 5000:5000
    networks:
      - antsk
    depends_on:
      - antskpg
    restart: always
    environment:
      - ASPNETCORE_URLS=http://*:5000
    volumes:
      - ./appsettings.json:/app/appsettings.json # Local configuration file needs to be placed in the same directory
      - D://model:/app/model
networks:
  antsk:
```
Taking this as an example, it means mounting the local D://model folder of Windows into the container /app/model. If so, the model address in your appsettings.json should be configured as
```
model/xxx.gguf
```

## 3️⃣Some meanings of configuration file
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
// Supports various databases, you can check SqlSugar, MySql, SqlServer, Sqlite, Oracle, PostgreSQL, Dm, Kdbndp, Oscar, MySqlConnector, Access, OpenGauss, QuestDB, HG, ClickHouse, GBase, Odbc, OceanBaseForOracle, TDengine, GaussDB, OceanBase, Tidb, Vastbase, PolarDB, Custom
DBConnection.DbType

// Connection string, need to use the corresponding string according to the different DB types
DBConnection.ConnectionStrings

//The type of vector storage, supporting Postgres, Disk, Memory, Qdrant, Redis, AzureAISearch
//Postgres and Redis require ConnectionString configuration
//The ConnectionString of Qdrant and AzureAISearch uses Endpoint | APIKey
KernelMemory.VectorDb

//Local model execution options: GPU and CPU. When using the online API, any option can be used.
LLamaSharp.RunType

//Local model path, used for quick selection of models under llama, as well as saving downloaded models.
LLamaSharp.FileDirectory

//Default admin account password
Login

//Import asynchronous processing thread count. A higher count can be used for online API, but for local models, 1 is recommended to avoid memory overflow issues.
BackgroundTaskBroker.ImportKMSTask.WorkerCount

```

## ⚠️Fixing Style Issues:
Run the following in AntSK/src/AntSK:
```
dotnet clean
dotnet build
dotnet publish "AntSK.csproj"
```
Then navigate to AntSK/src/AntSK/bin/Release/net8.0/publish and run:
```
dotnet AntSK.dll
```
The styles should now be applied after starting.

I'm using CodeFirst mode for the database, so as long as the database connection is properly configured, the table structure will be created automatically.

## ✔️Using llamafactory
```
1. First, ensure that Python and pip are installed in your environment. This step is not necessary if using an image, such as version v0.2.3.2, which already includes the complete Python environment.
2. Go to the model add page and select llamafactory.
3. Click "Initialize" to check whether the 'pip install' environment setup is complete.
4. Choose a model that you like.
5. Click "Start" to begin downloading the model from the tower. This may involve a somewhat lengthy wait.
6. After the model has finished downloading, enter http://localhost:8000/ in the request address. The default port is 8000.
7. Click "Save" and start chatting.
8. Many people ask about the difference between LLamaSharp and llamafactory. In fact, LLamaSharp is a .NET implementation of llama.cpp, but only supports local gguf models, while llamafactory supports a wider variety of models and uses Python implementation. The main difference lies here. Additionally, llamafactory has the ability to fine-tune models, which is an area we will focus on integrating in the future.
```

## 🤝 Contributing

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/AIDotNet/AntSK/pulls)

If you would like to contribute, feel free to create a [Pull Request](https://github.com/AIDotNet/AntSK/pulls), or give us [Bug Report](https://github.com/AIDotNet/AntSK/issues/new).


## 💕 Contributors

This project exists thanks to all the people who contribute.

<a href="https://github.com/AIDotNet/AntSK/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=AIDotNet/AntSK&max=1000&columns=15&anon=1" />
</a>

## 🚨  Use Protocol
This warehouse follows the [Apache-2.0 License]（ https://github.com/AIDotNet/AntSK?tab=Apache -2.0-1-ov file) open source protocol.
The Apache open source license allows the use of AntSK in commercial environments, provided that the license terms are followed. One of the main terms is to retain the copyright and license statements.
If you plan to use AntSK in commercial projects, you need to ensure that you follow the following steps:
1. Copyright statement containing Apache license. [Apache-2.0 License]（ https://github.com/AIDotNet/AntSK?tab=Apache -2.0-1-ov file).
2. If you modify the software source code, you need to clearly indicate these modifications in the source code.

## ☎️Contact Me
If you have any questions or suggestions, please contact me through my official WeChat account. We also have a discussion group where you can send a message to join, and then I will add you to the group.
![Official WeChat Account](https://github.com/AIDotNet/Avalonia-Assistant/blob/main/img/gzh.jpg)

---

We appreciate your interest in **AntSK** and look forward to collaborating with you to create an intelligent future!
