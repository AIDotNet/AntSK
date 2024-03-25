[简体中文](./README.md) | English
# AntSK

## An AI knowledge base/intelligent agent built with .Net 8+AntBlazor+SemanticKernel



## Core Features



- **Semantic Kernel**: Utilizes advanced natural language processing technologies to accurately understand, process, and respond to complex semantic queries, providing users with precise information retrieval and recommendation services.


- **Kernel Memory**: Capable of continuous learning and knowledge storage, AntSK has a long-term memory function, accumulating experience to offer more personalized interaction experiences.


- **Knowledge base**: Import knowledge into the database through documents (Word, PDF, Excel, Txt, Markdown, Json, PPT) and manage knowledge base documents.


- **GPTs Generation**：The platform supports the creation of personalized GPT models, try building your own GPT model.


- **API Interface Release**: Internal functions are provided as APIs for developers to integrate AntSK into other applications, enhancing application intelligence.


- **API Plugin System**: An open API plugin system allows third-party developers or service providers to easily integrate their services into AntSK, continuously enhancing application functions.


- **.Net Plugin System**: An open dll plugin system allows third-party developers or service providers to integrate their business functions into AntSK by generating dlls with the standard format codes, continuously enhancing application functions.


- **Internet Search**: AntSK can retrieve the latest information in real-time, ensuring that the information users receive is always timely and relevant.


- **Model management**: Adapts and manages different models from various manufacturers. It also supports offline running of models in 'gguf' format supported by llama.cpp.


- **National Information Creation**: AntSK supports domestic models and databases, and can operate under information creation conditions.



## Application scenarios



AntSK is suitable for various business scenarios, such as:

- Corporate knowledge management systems

- Automated customer service and chatbots

- Enterprise Search Engine

- Personalized recommendation system

- Intelligent assisted writing

- Education and online learning platform

- Other interesting AI applications



## Function example



First, you need to create a knowledge base

![Knowledge base](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93.png)



In the knowledge base, you can use documents or urls to import

![Knowledge base details](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93%E8%AF%A6%E6%83%85.png)



Click View to view the document slicing of the knowledge base

![Document Slice](https://github.com/AIDotNet/AntSK/blob/main/images/%E6%96%87%E6%A1%A3%E5%88%87%E7%89%87.png)



Then we need to create applications, which can create dialog applications and knowledge bases.

![Application](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%BA%94%E7%94%A8.png)



The application of knowledge base needs to select the existing knowledge base, which can be multiple

![Application Configuration](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%BA%94%E7%94%A8%E9%85%8D%E7%BD%AE.png)



Then you can ask questions about the knowledge base documents in the dialogue

![Q&A](https://github.com/AIDotNet/AntSK/blob/main/images/%E9%97%AE%E7%AD%94.png)



In addition, we can also create dialogue applications, and configure prompt word templates in corresponding applications

![Conversation application](https://github.com/AIDotNet/AntSK/blob/main/images/%E7%AE%80%E5%8D%95%E5%AF%B9%E8%AF%9D.png)



Let's see the effect

![Conversation effect](https://github.com/AIDotNet/AntSK/blob/main/images/%E5%AF%B9%E8%AF%9D%E6%95%88%E6%9E%9C.png)



## How do I get started?

Here I am using Postgres as a data and vector store, because the Semantic Kernel and Kernel Memory both support it, though you can switch to others.

The model by default supports local models in 'gguf' format from openai, azure openai, and llama. If you need to use other models, you can integrate them using the one-api.

The login configuration in the configuration file is the default account and password.

The following configuration files are needed:

## Using Docker Compose
A appsettings.json for the pg version and a simplified version (Sqlite+disk) docker-compose.simple.yml are provided.

Download docker-compose.yml from the project root directory, then place the configuration file appsettings.json in the same directory,

The pg image has already been prepared. You can modify the default account and password in the docker-compose.yml, and then your appsettings.json database connection needs to be consistent.

Then you can enter the directory and execute
```
docker compose up - d
```
to start AntSK.

How to mount local models and model download directories in docker

```
# Non-host version, does not use local proxy
version: '3.8'
services:
  antsk:
    container_name: antsk
    image: registry.cn-hangzhou.aliyuncs.com/AIDotNet/antsk:v0.2.1
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
      - ./appsettings.json:/app/appsettings.json # local configuration file must be placed in the same directory
      - D://model:/app/model
networks:
  antsk:

```
Using this as an example, the meaning is to mount the local folder D://model from Windows into the container /app/model. If so, your appsettings.json model directory should be configured as

```
model/xxx.gguf
```
Some meanings of the configuration file
// (The rest of the information is omitted as it's unnecessary for the translation example context.)

Solving the missing style issue:
Execute under AntSK/src/AntSK:
```
dotnet clean
dotnet build
dotnet publish "AntSK.csproj"
```
Then go to AntSK/src/AntSK/bin/Release/net8.0/publish
```
dotnet AntSK.dll
```

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
//Supports multiple databases, including SqlSugar, MySql, SqlServer, Sqlite, Oracle, PostgreSQL, Dm, Kdbndp, Oscar, MySqlConnector, Access, OpenGaussian, QuestDB, HG, ClickHouse, GBase, Odbc, OceanBaseForOracle, TDengine, GaussDB, OceanBase, Tidb, Vastbase, PolarDB, Custom
DBConnection DbType
//Connection string, corresponding strings need to be used according to different DB types
DBConnection ConnectionStrings
//The type of vector storage supports Postgres Disk Memory, where Postgres requires the configuration of ConnectionString
KernelMemory VectorDb
//The running mode used by the local model is GUP CPU. If using an online API, you can freely use one
LLamaSharp RunType
//The model path of the local session model should pay attention to distinguishing between Linux and Windows drive letters
LLamaSharp Chat
//The model path of the local vector model should pay attention to distinguishing between Linux and Windows drive letters
LLamaSharp Embedding
//Default administrator account password
Login
//The number of threads for importing asynchronous processing can be higher when using online APIs. Local models suggest 1, otherwise memory overflow and crash may occur
BackgroundTaskBroker ImportKMSTask WorkerCount

```


To learn more or start using**AntSK**, you can follow my public account and join the exchange group.



## Contact me

If you have any questions or suggestions, please follow my public account through the following ways, and send a message to me. We also have an exchange group, which can send messages such as joining the group, and then I will bring you into the exchange group

![Official account](https://github.com/AIDotNet/Avalonia-Assistant/blob/main/img/gzh.jpg)



---



We appreciate your interest in**AntSK**and look forward to working with you to create an intelligent future!
