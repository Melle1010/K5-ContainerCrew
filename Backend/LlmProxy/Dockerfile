FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Backend/LlmProxy/LlmProxy.csproj Backend/LlmProxy/
RUN dotnet restore Backend/LlmProxy/LlmProxy.csproj

COPY Backend/LlmProxy/. Backend/LlmProxy/
WORKDIR /src/Backend/LlmProxy
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LlmProxy.dll"]
