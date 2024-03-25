---
sidebar_position: 2
---

# 配置文件的一些含义
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