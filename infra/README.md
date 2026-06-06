# Azure deployment setup

This folder contains the Azure Container Apps deployment assets for the project:

- infra/main.bicep — creates the Azure Container Apps environment, ACR, Key Vault, and all three container apps.
- .github/workflows/deploy-aca.yml — builds the three Docker images and updates the Container Apps in Azure.

## Required GitHub repository variables

Set these in GitHub → Settings → Secrets and variables → Actions → Variables:

- AZURE_CLIENT_ID
- AZURE_TENANT_ID
- AZURE_SUBSCRIPTION_ID
- AZURE_RESOURCE_GROUP
- AZURE_LOCATION
- AZURE_ENVIRONMENT_NAME
- AZURE_ACR_NAME
- AZURE_KEYVAULT_NAME
- FRONTEND_APP_NAME
- LLM_PROXY_APP_NAME
- AI_CONTENT_APP_NAME

## First deployment

1. Create the GitHub variables above.
2. Make sure the Azure subscription has permission to create resource groups, ACR, Key Vault, and Container Apps.
3. Run the workflow manually from the Actions tab or push to main.

## Notes

- The Key Vault secrets are created with placeholder values. Replace them in Azure after the first deployment.
- The initial container app images are placeholder images; the workflow replaces them with the images built from your repository.
