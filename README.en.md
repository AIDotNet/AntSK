[简体中文](./README.md) | English
# AntSK
## AI Knowledge Base/Intelligent Agent built on .Net8+AntBlazor+SemanticKernel

## Core Features

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

## Application Scenarios

AntSK is suitable for various business scenarios, such as:
- Enterprise knowledge management system
- Automatic customer service and chatbots
- Enterprise search engine
- Personalized recommendation system
- Intelligent writing assistance
- Education and online learning platforms
- Other interesting AI Apps

## Function Examples
### Online Demo
```
https://antsk.ai-dotnet.com/
```
```
Default account: test

Default password: test

Due to the low configuration of the cloud server, the local model cannot be run, so the system settings permissions have been closed. You can simply view the interface. If you want to use the local model, please download and use it on your own.
```

### Other Function Examples
[Video Demonstration](https://www.bilibili.com/video/BV1zH4y1h7Y9/)

First, you need to create a knowledge base
![Knowledge Base](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93.png)

You can import documents or URLs into the knowledge base
![Knowledge Base Details](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93%E8%AF%A6%E6%83%85.png)Click to check the document slicing situation of the knowledge base
![Document slicing](https://github.com/AIDotNet/AntSK/blob/main/images/%E6%96%87%E6%A1%A3%E5%88%87%E7%89%87.png)

Then we need to create an application, which can be a dialogue application or a knowledge base.
![Application](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%BA%94%E7%94%A8.png)

For the knowledge base application, select the existing knowledge base, and multiple selections are possible
![Application configuration](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%BA%94%E7%94%A8%E9%85%8D%E7%BD%AE.png)

Then in the dialogue, questions can be asked about the documents in the knowledge base
![QA](https://github.com/AIDotNet/AntSK/blob/main/images/%E9%97%AE%E7%AD%94.png)

Additionally, we can create dialogue applications and configure prompt word templates in the corresponding application
![Dialogue application](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%AE%80%E5%8D%95%E5%AF%B9%E8%AF%9D.png)

Let's take a look at the effects below
![Dialogue effects](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%AF%B9%E8%AF%9D%E6%95%88%E6%9E%9C.png)

## How to get started?

Here I am using Postgres as the data and vector storage because Semantic Kernel and Kernel Memory support it, but you can also use other options.

The model by default supports the local model of openai, azure openai, and llama. If you need to use other models, you can integrate them using one-api.

The Login configuration in the configuration file is the default login account and password.

The following configuration file needs to be configured

## Using docker-compose

Provided the pg version **appsettings.json** and simplified version (Sqlite+disk) **docker-compose.simple.yml**

Download **docker-compose.yml** from the project root directory and place the configuration file **appsettings.json** in the same directory.

The pg image has already been prepared. You can modify the default username and password in docker-compose.yml, and then the database connection in your **appsettings.json** needs to be consistent.

Then you can execute the following command in the directory to start AntSK
```
docker-compose up -d
```

## How to mount local models and model download directory in docker
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

## Some meanings of configuration file
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
DBConnection.ConnectionStrings//Vector storage types, supporting Postgres, Disk, and Memory. Postgres requires configuring a ConnectionString.
KernelMemory.VectorDb

//Local model execution options: GPU and CPU. When using the online API, any option can be used.
LLamaSharp.RunType

//Model path for local sessions. Note the distinction in file paths between Linux and Windows drives.
LLamaSharp.Chat

//Model path for local vector models. Note the distinction in file paths between Linux and Windows drives.
LLamaSharp.Embedding

//Local model path, used for quick selection of models under llama, as well as saving downloaded models.
LLamaSharp.FileDirectory

//Default admin account password
Login

//Import asynchronous processing thread count. A higher count can be used for online API, but for local models, 1 is recommended to avoid memory overflow issues.
BackgroundTaskBroker.ImportKMSTask.WorkerCount

```

## Fixing Style Issues:
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

## Using llamafactory
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

To learn more or get started with **AntSK**, follow my official WeChat account and join the discussion group.

## Contact Me
If you have any questions or suggestions, please contact me through my official WeChat account. We also have a discussion group where you can send a message to join, and then I will add you to the group.
![Official WeChat Account](https://github.com/AIDotNet/Avalonia-Assistant/blob/main/img/gzh.jpg)

---

We appreciate your interest in **AntSK** and look forward to collaborating with you to create an intelligent future!
