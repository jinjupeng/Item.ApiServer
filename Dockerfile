#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ApiServer/ApiServer.csproj", "ApiServer/"]
COPY ["ApiServer.Common/ApiServer.Common.csproj", "ApiServer.Common/"]
RUN dotnet restore "ApiServer/ApiServer.csproj"
COPY . .
WORKDIR "/src/ApiServer"
RUN dotnet build "ApiServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiServer.dll"]