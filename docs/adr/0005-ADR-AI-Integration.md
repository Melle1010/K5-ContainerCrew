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
* Secrets are never exposed to the frontend or to Service A.
* Failures from the LLM provider are handled in one place, making the system more stable.
* Service A remains lightweight and focused on application logic.
* Logging and monitoring of AI calls become easier and more consistent.

### Negative consequences:
* The communication between Service A and Service B must be well‑defined to avoid inconsistencies.
* Centralized error handling can make it harder to trace where a failure originated unless logging and correlation are implemented carefully.

## Follow-up
We will evaluate the decision based on whether AI‑related failures are isolated to Service B, whether Key Vault access remains secure and traceable, and whether the frontend consistently receives stable responses even during external API issues.
The decision will be revisited if the AI provider changes, if latency becomes a problem, or if the architecture evolves to require fewer services.
