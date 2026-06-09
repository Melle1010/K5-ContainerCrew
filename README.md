# Fullstack Deployment – ContainerCrew

This project is an educational upgrade that combines two previously separate applications: one frontend project and one backend project. The goal of this version is to integrate both parts into a single fullstack system, first locally through containerization and then in a production environment using Azure. The local integration is done by containerizing both applications and running them together through Docker, with all sensitive values stored in a .env file that is excluded from the repository.
The production‑level integration is deployed in Azure using Azure Container Apps, Azure Key Vault for secure secret management, and Log Analytics for monitoring and troubleshooting. CI/CD automates build, test, and deployment to ensure consistent and reliable releases.

## Gemini & Local Docker Setup

This project uses Gemini as the LLM provider, which means the application requires a Gemini API key both locally and in production. To run the system locally, you must first generate a personal Gemini API key at:
```
https://aistudio.google.com/app/apikey
```
Sign in with your Google account, create an API key, and copy it.
After generating the key, create a .env file in the root folder (the same level as docker-compose.yml). This file is used by Docker to inject all required secrets into the containers. Use the following .env template:

```
SECRET_API_KEY=super-secret-key-123
GEMINI_API_KEY=YOUR_GEMINI_API_KEY
```

After this step you can run the following syntax to create both images and containers.

```
docker compose up --build
```

Once the containers are running, the applications are available at:

Frontend - http://localhost:8080/
Backend - http://localhost:5292/scalar/

## Broken deploy troubleshooting

A broken deployment is diagnosed entirely with the help of information stored in the ContainerAppConsoleLogs_CL table, which can be found inside of the Log Analytics workspace in Azure Portal. For our use case, we have identified three query views that are especially important. The first is the ability to see which HTTP requests were made. This helps with the confirmation that the application is reachable and that routing and ingress behave as expected. The second is the exception view. This one exposes any unhandled errors returned to the user and makes it possible to pinpoint logic issues, configuration problems, or dependency failures. The third is the execution‑time & logged message view, which shows how long different stages of the code take to run and a small message to clarify the position in the code. The following three queries represent each of these views respectively and can be used as an input directly into the Logs section inside of the Log Analytics workspace. Make sure to use KQL mode.

View 1 - POST Http requests
```
ContainerAppConsoleLogs_CL
| where Log_s contains "GET" or Log_s contains "POST"
| sort by TimeGenerated desc
```

View 2 - Exceptions
```
ContainerAppConsoleLogs_CL
| where Log_s contains "Exception"
| sort by TimeGenerated desc
```

View 3 - Execution time & Logged message
```
ContainerAppConsoleLogs_CL
| where Log_s contains "LOG:"
| project TimeGenerated, Log_s
| sort by TimeGenerated asc
```
