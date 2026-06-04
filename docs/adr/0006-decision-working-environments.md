## Title
Environment Strategy for Local Development and Production Deployment

## Metadata
* Date: 2026‑06‑01
* Status: Accepted
* Decision Maker: Ivan
* Related: Fullstack Deployment Assignment (K3+K4)

## Context
Our full‑stack system must operate in multiple environments.

## Decision
We define two environments: local and production.

The local environment is used during development. All services run on the developer machine, either directly with dotnet run or in local containers. Local configuration and secrets are injected through environment variables, which remain outside the repository and are excluded from all Docker builds. The goal is to allow developers to start the system quickly and test features without depending on Azure.
The production environment runs in Azure. The backend is deployed to Azure Container Apps, the frontend is hosted in Azure, and all secrets are stored in Azure Key Vault. Production is updated only through the CI/CD pipeline, not manually. This environment is the one used for external access and monitoring.

## Motivation
Local and production need to be separated so that development does not depend on cloud resources and production does not depend on manual steps.
Local supports fast iteration.
Production supports secure deployment, managed identities, Key Vault, and monitoring.

## Consequences

### Positive consequences:
* Developers can run the system without Azure access.
* Production stays consistent and secure because it is only updated through CI/CD.
* Docker containerization + Azure Key Vault reduces the attack area the application can experience from potential hackers.

### Negative consequences:
* Using Azures student subscription
