# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/AntSK/AntSK.csproj", "AntSK/"]
RUN dotnet restore "AntSK/AntSK.csproj"

# Copy everything else and build
COPY src/ .
WORKDIR "/src/AntSK"
RUN dotnet build "AntSK.csproj" -c Release -o /app/build
RUN dotnet publish "AntSK.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /service
EXPOSE 5000

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone
RUN apt update && apt install -y libpugixml-dev libtbb-dev
ENTRYPOINT ["dotnet", "AntSK.dll"]
