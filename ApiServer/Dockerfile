#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p mypassword123 # Windows using Linux containers
# RUN dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p mypassword123 # macOS or Linux
# RUN dotnet dev-certs https --trust
COPY ["ApiServer/ApiServer.csproj", "ApiServer/"]
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