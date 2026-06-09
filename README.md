# Fullstack Deployment – ContainerCrew

This project is an educational upgrade that combines two previously separate applications: one frontend project and one backend project. The goal of this version is to integrate both parts into a single fullstack system, first locally through containerization and then in a production environment using Azure. The local integration is done by containerizing both applications and running them together through Docker, with all sensitive values stored in a .env file that is excluded from the repository.
The production‑level integration is deployed in Azure using Azure Container Apps, Azure Key Vault for secure secret management, and Log Analytics for monitoring and troubleshooting. CI/CD automates build, test, and deployment to ensure consistent and reliable releases.

## Architecture Overview

The system consists of three containerized services that run together both locally and in production:
* Frontend – The user‑facing application served as static files.
* AI Content API – The main backend service that exposes the application’s API and forwards AI‑related requests to the LLM Proxy.
* LLM Proxy API – A backend service responsible for communicating with Gemini, the chosen LLM for this project.

## AI driven functionality

The AI‑driven functionality is built around Gemini, which is used to generate content based on user input. The frontend sends a prompt to the AI Content API, which validates the request and forwards it to the LLM Proxy. The LLM Proxy then communicates directly with Gemini by using an API key. When it receives the generated text it returns it to the AI Content API which in turn sends the response back to the user in the frontend. This architechture ensures separation of concerns while also making it easy to observe how data flows through the system. The AI feature is fully functional both locally and in production.

%% Endpoints

The backend has two endpoints, one GET endpoint which is just a helper and can be used to determine the version of Gemini currently being used in case the current one has been phased out. And the second POST endpoint, the most important one, is responsible for Ai content generation. It accepts a user prompt from the frontend and returns the Ai generated response after it has passed through the AI Content API and the LLM Proxy.

## CI/CD Pipeline

The CI/CD pipeline is responsible for building, testing, and deploying the entire system in a consistent and automated way. Every change pushed to the repository triggers a workflow that restores dependencies, compiles the backend services, and runs all automated tests to ensure that the application behaves as expected before any deployment takes place. When changes are merged into the main branch, the pipeline builds fresh Docker images for all services, tags them with both the commit SHA and a latest tag, and pushes them to Azure Container Registry. After the images are published, the pipeline updates the running Azure Container Apps environment so that the new versions of the services are deployed without manual intervention. This process ensures that the system remains reproducible, traceable, and stable across releases, while also reducing the risk of configuration drift or human error during deployment.

## Secret Handling
Secret handling is done with two separate methods determined by the working environment. Locally, all sensitive values are stored in a .env file that is excluded from version control and injected into the containers at runtime, allowing the system to run with the required configuration without exposing any secrets publicly. In production, the same values are stored securely in Azure Key Vault, and the services retrieve them using a system‑assigned managed identity, which removes the need for storing credentials anywhere in the codebase or pipeline. This setup ensures that only the services that require access to specific secrets can retrieve them, following the principle of least privilege. By keeping the configuration model identical across environments, the application behaves the same locally and in Azure.

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
