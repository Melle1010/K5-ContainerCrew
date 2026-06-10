# ADR CI/CD Pipeline Design (Triggers, Steps, Quality Gates)

## Title
CI/CD Pipeline Design for Automated Build, Test, Image Publishing, and Deployment to Azure

## Metadata
* Date: 2026‑06‑01
* Status: Accepted
* Decision Maker: Ivan
* Related: Fullstack Deployment Assignment (K3+K4)

## Context
Our full‑stack application requires a production‑like delivery workflow where code is automatically built, tested, containerized, and deployed to Azure. 
The assignment also requires traceability, quality gates, and secure handling of secrets. 
We need a CI/CD setup that supports two backend services, tests, a separate frontend and Azure Container Apps as the hosting platform.

## Decision
We use a two‑pipeline CI/CD setup in GitHub Actions:
* A CI pipeline that runs on every push and pull request to both dev and main. It builds the backend services and runs all automated tests.
* A CD pipeline that runs only on pushes to main. It builds container images, pushes them to Azure Container Registry and deploys the services to Azure Container Apps.

## Motivation
This separation ensures that all code is validated before merging, and only verified code is deployed. 
It also aligns with real‑world DevOps practices and the assignment’s requirement for clear quality gates. 
Deployment is predictable, traceable, and secure through Azure key vault.

## Consequences

### Positive consequences:
* Only tested and validated code reaches production.
* Deployment is fully automated and reproducible.
* Image tagging with commit SHA provides strong traceability.
* No secrets are stored in the code nor in the pipeline due to Azure Key Vault.

### Negative consequences:
* Azure integration takes time.

## Follow-up
* The CI pipeline consistently catches build or test failures before code is merged.
* The CD pipeline deploys only when the CI pipeline is green, ensuring production remains stable.
* The Azure environment always reflects the state of the main branch without manual intervention.
