# 0001 — Host workloads on Azure Container Apps

**Date:** 2026-06-01 | **Status:** Accepted | **Owner:** Dev team

---

## Context

We needed a hosting platform in Azure that scales with usage, and can run frontends, APIs, and background workers in one place, without paying for idle resources.

---

## Decision

Use **Azure Container Apps (ACA)** for all workloads. Everything runs as Docker containers in a shared Container Apps Environment.

---

## Alternatives

| | Azure App Service | Azure Static Web Apps | **Azure Container Apps** |
|---|---|---|---|
| Containers | Optional | No | ✅ Yes |
| Scale to zero | No | API only | ✅ Yes |
| Background workers | Limited | ❌ No | ✅ Yes |
| Cost model | Per plan (always on) | Free/Standard | Pay per use |

**App Service** — rejected: you pay even when idle, limited microservice support.  
**Static Web Apps** — rejected: backend locked to Azure Functions, no arbitrary containers.

---

## Consequences

**Good:**
- Pay-per-second billing; scale to zero in dev/staging cuts costs significantly.
- No cluster management overhead.
- Blue/green deploys and traffic splitting out of the box.
- Key Vault + Managed Identity keeps secrets out of code.

**Watch out for:**
- Everything must be Dockerized — team needs to maintain Dockerfiles and ACR.
- Slightly more setup upfront than a zip deploy.

---

## Follow-up

Re-evaluate after 3 months in production. Key things to track: monthly cost vs. estimate, deploy time (target < 5 min), and scale-to-zero actually firing in non-prod.
