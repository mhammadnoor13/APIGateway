# syntax=docker/dockerfile:1

########################
# Stage-0 – runtime base
########################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

########################
# Stage-1 – build
########################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Gateway/Gateway.csproj", "src/Gateway/"]
RUN dotnet restore "src/Gateway/Gateway.csproj"
COPY . .
WORKDIR /src/src/Gateway
RUN dotnet build "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

########################
# Stage-2 – publish
########################
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.csproj" -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

########################
# Stage-3 – final image
########################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Gateway.dll"]
