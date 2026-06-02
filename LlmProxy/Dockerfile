# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files
COPY ["AiContent/AiContent.csproj", "AiContent/"]
COPY ["LlmProxy/LlmProxy.csproj", "LlmProxy/"]

# Restore dependencies
RUN dotnet restore "LlmProxy/LlmProxy.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/LlmProxy"
RUN dotnet build "LlmProxy.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LlmProxy.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080

ENTRYPOINT ["dotnet", "LlmProxy.dll"]
