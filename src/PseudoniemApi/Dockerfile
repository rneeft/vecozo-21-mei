FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/PseudoniemApi/PseudoniemApi.csproj", "src/PseudoniemApi/"]
COPY NuGet.config .
RUN dotnet restore "src/PseudoniemApi/PseudoniemApi.csproj"

COPY src/Data/ src/Data/
COPY src/PseudoniemApi/ src/PseudoniemApi/
RUN dotnet publish "src/PseudoniemApi/PseudoniemApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PseudoniemApi.dll"]
