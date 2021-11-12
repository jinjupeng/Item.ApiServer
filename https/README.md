# Create SSL Certificate

[https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-6.0](https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https)

## Windows using Linux containers

Generate certificate and configure local machine:

```powershell
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p mypassword123
dotnet dev-certs https --trust
```

## macOS or Linux

Generate certificate and configure local machine:

```powershell
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p mypassword123
dotnet dev-certs https --trust
```

## Windows using Windows containers

Generate certificate and configure local machine:

```powershell
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p mypassword123
dotnet dev-certs https --trust
```