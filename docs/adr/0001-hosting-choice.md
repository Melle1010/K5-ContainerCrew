## 0001-hosting-choice

Host application workloads on Azure Container Apps

---

## Metadata

- **Date:** 2026-06-01
- **Status:** Accepted
- **Decision-makers:** Development team

---

## Context (the problem being solved)

- We need to select a primary hosting infrastructure for our application components within the Microsoft Azure ecosystem.
- The solution must balance scalability, cost efficiency, ease of deployment, and long-term architectural flexibility.
- Requirements and constraints:
  - **Cost:** Non-production environments and variable production workloads require cost optimization — pay only for actual usage.
  - **Expertise:** The team should not need to manage Kubernetes clusters at a low level (nodes, control planes, node pools).
  - **Containerization:** The architecture assumes all components can be Dockerized.
  - **Flexibility:** The solution must support web frontends, backend APIs, and background workers within a single unified environment.

---

## Decision (what we are doing)

We will use **Azure Container Apps (ACA)** as the primary hosting platform for our application workloads.

- **What is included:** Web frontends, backend APIs, and background workers running as Dockerized containers within a shared Container Apps Environment.
- **What is not included:** Management of underlying Kubernetes clusters, nodes, or control planes — this is abstracted away by ACA.

---

## Alternatives evaluated

### Alternative A — Azure App Service

- **Advantages:**
  - Simple configuration; supports zip deploy without containerization.
  - Predictable cost model based on App Service Plan.
  - Mature service with extensive documentation and community support.
- **Disadvantages:**
  - Requires continuous payment for dedicated underlying VM instances, even under low load.
  - Limited support for microservice patterns and service discovery.
  - No native scale-to-zero without specialized plans (Premium/Consumption).
- **Why was it rejected?** The cost model does not fit variable workloads and non-production environments. Limited flexibility for future architectural evolution.

---

### Alternative B — Azure Static Web Apps (SWA)

- **Advantages:**
  - Very cost-effective (free/standard tier) for static SPA frontends.
  - Built-in global CDN distribution and automatic HTTPS.
  - Scale-to-zero for the API tier (via Azure Functions).
- **Disadvantages:**
  - Backend functionality is restricted to Azure Functions — no long-running processes or arbitrary containers.
  - Does not support background workers, containers, or alternative backend frameworks.
  - Does not provide a unified environment for all components.
- **Why was it rejected?** Too restrictive for backend workloads. Forces a dependency on Azure Functions that limits architectural freedom.

---

### Alternative C — Azure Container Apps (ACA) ✅ Chosen

- **Advantages:**
  - Serverless and KEDA-based — supports scaling to zero instances during inactivity.
  - Abstracts Kubernetes complexity: we get Kubernetes-level resilience without managing clusters.
  - Supports any containerized language, framework, or long-running background worker process.
  - Built-in support for blue/green deployments and traffic splitting.
  - Web frontends, APIs, and background workers can coexist within the same Container Apps Environment.
- **Disadvantages:**
  - All components must be Dockerized, requiring the team to maintain Dockerfiles and a container registry (Azure Container Registry).
  - Slightly higher initial configuration complexity compared to a simple zip deploy on App Service.
- **Why was it rejected?** It was *not* rejected — this is the chosen alternative.

---

## Consequences

- **Technical consequences:**
  - All application components must be packaged as Docker images and published to Azure Container Registry (ACR).
  - Internal service-to-service calls are handled via the Container Apps Environment's internal DNS and service discovery.
  - Dapr integration is available if needed for future microservice patterns.

- **Operations & support:**
  - Deployments are managed via GitHub Actions and CI/CD pipelines targeting ACR and ACA.
  - Revision management in ACA enables blue/green deployments and straightforward rollback.
  - Monitoring is handled via Azure Monitor and Log Analytics connected to the Container Apps Environment.

- **Security & compliance:**
  - Secrets and configuration values are managed via Azure Key Vault with Managed Identity — no hardcoded credentials.
  - Network isolation is configured at the environment level; internal traffic is not exposed publicly.
  - All images should be scanned for known vulnerabilities via ACR Tasks or an equivalent scanning tool.

- **Cost:**
  - Cost drivers: number of active replicas, CPU/memory consumption per second, and network traffic.
  - Scale-to-zero in non-production environments is expected to significantly reduce ongoing costs compared to an App Service Plan.
  - Cost tracking is done via Azure Cost Management with per-environment tagging (dev/staging/prod).

- **Team consequences:**
  - The team needs basic Docker proficiency and an understanding of container registries.
  - Onboarding of new team members includes a walkthrough of the Dockerfile structure and CI/CD pipeline.
  - Reduced DevOps overhead compared to raw AKS — the team does not need to manage cluster operations.
  - Deployment time from push to live revision (target: < 5 min).
  - Uptime and error rate via Azure Monitor alerts.
  - Scale-to-zero behavior in dev/staging — verify that instances actually scale down during inactivity.

- **When should the decision be re-evaluated?**
  - After 3 months in production for an initial cost and operations review.
  - If there is a significant change in team size, traffic volume, or architecture (e.g. a need for full AKS-level control).
