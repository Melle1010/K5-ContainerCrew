# ADR 0003 safe configuration handling

## Title
Handle API-keys and secrets without hard coding them. Follow the principle about "Least Priviledge" for access between services.

## Metadata
- **Date:** 2026-06-01
- **Status:** Accepted
- **Beslutsfattare:** Vivienne Bengtsson
- **Relaterat:** 

## Context 
- We need a safe way on handling API-keys and secrets without hard coding them,
- We need to follow the principles about "Least Priviledge" for access between services. 

## Decision
We're using Azure Key Vault for storing secrets and Manage Identity (User-Assigned) to give the application access to the vault with no password.

## Alternatices considered

### Alternative A: Environment variables in Azure Container Apps
- **Advantages:** Easy to set up.
- **Disadvantages:** Secrets are visible in Azure-Portal.
- **Why wasn't it selected?** Visible secrets is a security risk.

### Alternative B: Azure Key Vault with Managed Identity
- **Advantages:** No passwords in the code, centralized logging of who reads secrets.
- **Disadvantages:** Requries more code (SDK) and configuration in Azure.
- **Why was it selected?** It's the safest method and eliminates risk for leaked login credentials.

## Follow-up
- We will check that no secrets are in the source code and that Managed Identity-logs shows correct calls.
