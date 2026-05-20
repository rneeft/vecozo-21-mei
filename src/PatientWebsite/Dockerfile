FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/PatientWebsite/PatientWebsite.csproj", "src/PatientWebsite/"]
COPY NuGet.config .
RUN dotnet restore "src/PatientWebsite/PatientWebsite.csproj"

COPY src/Data/ src/Data/
COPY src/PatientWebsite/ src/PatientWebsite/
RUN dotnet publish "src/PatientWebsite/PatientWebsite.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PatientWebsite.dll"]
