[简体中文](./README.md) | English
# AntSK

## Based on AI knowledge base/agent created by Net8+AntBlazor+SemanticKernel



## Core functions



- **Semantic Kernel**: It uses advanced natural language processing technology to accurately understand, process and respond to complex semantic queries, and provides users with accurate information retrieval and recommendation services.



- **Kernel Memory**: It has the ability to continuously learn and store knowledge points. AntSK has a long-term memory function to accumulate experience and provide a more personalized interactive experience.



- **Knowledge base**: Knowledge base documents can be created by importing knowledge base documents (Word, PDF, Excel, Txt, Markdown, Json, PPT) and other forms.



- **API plug-in system**: an open API plug-in system that allows third-party developers or service providers to easily integrate their services into AntSK and continuously enhance application functions.



- **Online search**: AntSK can obtain the latest information in real time to ensure that the information received by users is always the most timely and relevant.



- **GPTs generation**: This platform supports the creation of personalized GPT models and attempts to build your own GPT models.



- **API interface publishing**: internal functions are provided externally in the form of API, so that developers can easily translate Xzy AntSK KnowledgeBase is integrated into other applications to enhance application intelligence.

- **Model management**: Adapt and manage different models from different vendors.



## Application scenarios



AntSK is applicable to a variety of business scenarios, such as:

- Enterprise level knowledge management system

- Automatic customer service and chat robot

- Enterprise Search Engine

- Personalized recommendation system

- Intelligent assisted writing

- Education and online learning platform

- Other interesting AI Apps



## Function example



First, you need to create a knowledge base

![Knowledge base](https://github.com/xuzeyu91/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93.png)



In the knowledge base, you can use documents or urls to import

![Knowledge base details](https://github.com/xuzeyu91/AntSK/blob/main/images/%E7%9F%A5%E8%AF%86%E5%BA%93%E8%AF%A6%E6%83%85.png)



Click View to view the document slicing of the knowledge base

![Document Slice](https://github.com/xuzeyu91/AntSK/blob/main/images/%E6%96%87%E6%A1%A3%E5%88%87%E7%89%87.png)



Then we need to create applications, which can create dialog applications and knowledge bases.

![Application](https://github.com/xuzeyu91/AntSK/blob/main/images/%E5%BA%94%E7%94%A8.png)



The application of knowledge base needs to select the existing knowledge base, which can be multiple

![Application Configuration](https://github.com/xuzeyu91/AntSK/blob/main/images/%E5%BA%94%E7%94%A8%E9%85%8D%E7%BD%AE.png)



Then you can ask questions about the knowledge base documents in the dialogue

![Q&A](https://github.com/xuzeyu91/AntSK/blob/main/images/%E9%97%AE%E7%AD%94.png)



In addition, we can also create dialogue applications, and configure prompt word templates in corresponding applications

![Conversation application](https://github.com/xuzeyu91/AntSK/blob/main/images/%E7%AE%80%E5%8D%95%E5%AF%B9%E8%AF%9D.png)



Let's see the effect

![Conversation effect](https://github.com/xuzeyu91/AntSK/blob/main/images/%E5%AF%B9%E8%AF%9D%E6%95%88%E6%9E%9C.png)



## How do I get started?

Login is the default login account and password

Here I use Postgres as data storage and vector storage, because both the Semantic Kernel and Kernel Memory support it. Of course, you can switch to other ones.

The model supports openai by default. If you need to use azure openai and need to adjust the dependency injection of SK, you can also use one api for integration.

The following configuration files need to be configured

## Using Docker Compose
Provided pg version appsettings. json and simplified version (Sqlite+disk) Docker Compose. simple. yml
Download Docker Compose.yml from the project root directory, and then place the configuration file appsettings.json and it in a unified directory,
The image of PG has been prepared here. You can modify the default account password in Docker Compose.yml, and your appsettings. json database connection needs to be consistent.
Then you can enter the directory and execute it
```
docker compose up - d
```
To start AntSK

Some meanings of configuration files

```

{
  "DBConnection": {
    "DbType": "Sqlite", 
    "ConnectionStrings": "Data Source=AntSK.db;"
  },
  "OpenAIOption": {
    "EndPoint": "http://localhost:5000/llama/",
    "Key": "NotNull",
    "Model": "gpt4-turbo",
    "EmbeddingModel": "text-embedding-ada-002"
  },
  "KernelMemory": {
    "VectorDb": "Disk", 
    "ConnectionString": "Host=;Port=;Database=antsk;Username=;Password=",
    "TableNamePrefix": "km-"
  },
  "LLamaSharp": {
    "RunType": "GPU", 
    "Chat": "D:\\Code\\AI\\AntBlazor\\model\\qwen1_5-1_8b-chat-q8_0.gguf",
    "Embedding": "D:\\Code\\AI\\AntBlazor\\model\\qwen1_5-1_8b-chat-q8_0.gguf"
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
//You can use an online API that conforms to the OpenAI format (domestic models use one API adapter), or you can use AntSK's built-in llama API, with the IP and port being the AntSK startup address
OpenAIOption EndPoint
//Model key, if using a local model, it can default to Notnull. Chinese cannot be used here
OpenAIOption Key
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

![Official account](https://github.com/xuzeyu91/Avalonia-Assistant/blob/main/img/gzh.jpg)



---



We appreciate your interest in**AntSK**and look forward to working with you to create an intelligent future!
