---
sidebar_position: 1
---

# 如何开始？

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

DB我使用的是CodeFirst模式，只要配置好数据库链接，表结构是自动创建的