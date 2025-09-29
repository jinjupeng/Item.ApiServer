# 使用官方的 .NET 8.0 运行时作为基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 使用官方的 .NET 8.0 SDK 作为构建镜像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件并还原依赖项
COPY ["ApiServer.WebApi/ApiServer.WebApi.csproj", "ApiServer.WebApi/"]
COPY ["ApiServer.Application/ApiServer.Application.csproj", "ApiServer.Application/"]
COPY ["ApiServer.Domain/ApiServer.Domain.csproj", "ApiServer.Domain/"]
COPY ["ApiServer.Infrastructure/ApiServer.Infrastructure.csproj", "ApiServer.Infrastructure/"]
COPY ["ApiServer.Shared/ApiServer.Shared.csproj", "ApiServer.Shared/"]

RUN dotnet restore "ApiServer.WebApi/ApiServer.WebApi.csproj"

# 复制所有源代码并构建应用程序
COPY . .
WORKDIR "/src/ApiServer.WebApi"
RUN dotnet build "ApiServer.WebApi.csproj" -c Release -o /app/build

# 发布应用程序
FROM build AS publish
RUN dotnet publish "ApiServer.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 最终阶段：创建运行时镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 创建非root用户
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "ApiServer.WebApi.dll"]
