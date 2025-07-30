
# 1) Use full SDK so we have dotnet-watch and build tools
FROM mcr.microsoft.com/dotnet/sdk:8.0

# 2) Set working directory where we'll mount your code
WORKDIR /src

# 3) Ensure reliable file‑watching on a Windows bind mount
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

# 4) Tell ASP.NET to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY ../scripts/wait-for-rabbit.sh /wait-for-rabbit.sh
RUN chmod +x /wait-for-rabbit.sh

# 5) On container start, run dotnet-watch against your API project
ENTRYPOINT ["/wait-for-rabbit.sh", "rabbitmq","dotnet", "watch", "run", "--project", "Gateway/src/Gateway/Gateway.csproj", "--urls", "http://+:8080"]
