FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Backend/AiContent/AiContent.csproj Backend/AiContent/
RUN dotnet restore Backend/AiContent/AiContent.csproj

COPY Backend/AiContent/. Backend/AiContent/
WORKDIR /src/Backend/AiContent
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AiContent.dll"]
