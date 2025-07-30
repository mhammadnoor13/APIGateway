# ------------------------------------------------------------
# 1) Runtime base image
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ------------------------------------------------------------
# 2) Build stage
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 2.1) Copy shared Contracts project for ProjectReference
COPY ["Common/Contracts/Contracts.Shared/Contracts.Shared.csproj", "Common/Contracts/Contracts.Shared/"]

# 2.2) Copy Gateway csproj for restore
COPY ["Gateway/src/Gateway/Gateway.csproj", "Gateway/src/Gateway/"]

# 2.3) Restore (pulls in the shared Contracts)
RUN dotnet restore "Gateway/src/Gateway/Gateway.csproj"

# 2.4) Copy all your source code (Gateway + shared)
COPY . .

# 2.5) Publish the Gateway
WORKDIR "/src/Gateway/src/Gateway"
RUN dotnet publish "Gateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ------------------------------------------------------------
# 3) Final runtime image
# ------------------------------------------------------------
FROM base AS final
WORKDIR /app

# 3.1) Copy the wait-for-rabbit script and make it executable
COPY ["scripts/wait-for-rabbit.sh", "/wait-for-rabbit.sh"]
RUN chmod +x /wait-for-rabbit.sh

# 3.2) Copy published output
COPY --from=build /app/publish .

# 3.3) Use the wrapper to block until RabbitMQ is ready
ENTRYPOINT ["/wait-for-rabbit.sh", "rabbitmq", "dotnet", "Gateway.dll"]
