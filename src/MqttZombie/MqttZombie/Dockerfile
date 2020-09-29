FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# ENV MSBUILDSINGLELOADCONTEXT=1

COPY . .
RUN dotnet restore "MqttZombie.csproj" --ignore-failed-sources

# WORKDIR "src/Backend/WebApi/"
RUN dotnet build "MqttZombie.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MqttZombie.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
RUN apk --no-cache add curl
HEALTHCHECK CMD curl --fail http://localhost/health || exit 1
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MqttZombie.dll"]
