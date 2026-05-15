FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
RUN apt-get update && apt-get install -y ca-certificates libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Account/Account.csproj", "Account/"]
COPY ["src/Common/Common.csproj", "Common/"]
COPY ["src/Incidents/Incidents.csproj", "Incidents/"]
COPY ["src/CommunityIncidentReportApp/CommunityIncidentReportApp.csproj", "CommunityIncidentReportApp/"]
RUN dotnet restore "./CommunityIncidentReportApp/CommunityIncidentReportApp.csproj"
COPY ["src/", "."]
WORKDIR "/src/CommunityIncidentReportApp"
RUN dotnet build "./CommunityIncidentReportApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommunityIncidentReportApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "CommunityIncidentReportApp.dll" ]