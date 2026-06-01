## Title
AI Integration Design: Service Responsibilities, Error Handling, and Secure Key Management

## Metadata
* Date: 2026‑06‑01
* Status: Accepted
* Decision Maker: Ivan
* Related: Fullstack Deployment Assignment (K3+K4)

## Context
Our application must include an AI‑driven feature that communicates with an external LLM provider.
The assignment requires that all secrets are stored in Azure Key Vault, that the integration is secure, and that the architecture supports clear separation of concerns.
We also need predictable error handling so the frontend receives stable and user‑friendly responses even when the AI provider fails or returns low‑quality output.

## Decision
We split the AI integration into two backend services:
* Content API (Service A) receives requests from the frontend, validates input, and prepares the prompt.
* LLM Proxy API (Service B) retrieves the API key from Azure Key Vault using Managed Identity and performs the actual call to the LLM provider.

## Motivation
This separation ensures that secrets remain isolated in a single service, reduces the attack surface, and keeps the frontend shielded from external API failures.
It also makes the system easier to test, monitor, and scale independently.
By centralizing the LLM call in Service B, we gain a predictable integration point with consistent error handling and logging.

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
