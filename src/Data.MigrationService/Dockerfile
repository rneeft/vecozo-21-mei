FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/Data.MigrationService/Data.MigrationService.csproj", "src/Data.MigrationService/"]
COPY NuGet.config .
RUN dotnet restore "src/Data.MigrationService/Data.MigrationService.csproj"

COPY src/Data/ src/Data/
COPY src/Data.MigrationService/ src/Data.MigrationService/
RUN dotnet publish "src/Data.MigrationService/Data.MigrationService.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Data.MigrationService.dll"]
