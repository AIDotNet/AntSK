FROM mcr.microsoft.com/dotnet/aspnet:8.0  AS base
WORKDIR /service
EXPOSE 5000

WORKDIR /app
COPY ["AntSK/bin/Release/net8.0/publish", "publish"]

WORKDIR /app/publish

FROM base AS final
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone
ENTRYPOINT ["dotnet", "AntSK.dll"]