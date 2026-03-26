FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NightmaresWiki2.sln ./
COPY NuGet.Config ./
COPY src/NightmaresWiki.Api/NightmaresWiki.Api.csproj src/NightmaresWiki.Api/

RUN dotnet restore src/NightmaresWiki.Api/NightmaresWiki.Api.csproj --configfile NuGet.Config

COPY . .

RUN dotnet publish src/NightmaresWiki.Api/NightmaresWiki.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "NightmaresWiki.Api.dll"]
