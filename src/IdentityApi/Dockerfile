FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/IdentityApi/IdentityApi.csproj", "src/IdentityApi/"]
COPY NuGet.config .
RUN dotnet restore "src/IdentityApi/IdentityApi.csproj"

COPY src/Data/ src/Data/
COPY src/IdentityApi/ src/IdentityApi/
RUN dotnet publish "src/IdentityApi/IdentityApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityApi.dll"]
