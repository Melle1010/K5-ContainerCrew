@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Name of the Azure Container Apps environment.')
param environmentName string = 'containercrew-env'

@description('Name of the Azure Container Registry (must be globally unique).')
param containerRegistryName string = 'containercrewacr${uniqueString(resourceGroup().id)}'

@description('Name of the Key Vault (must be globally unique).')
param keyVaultName string = take('cckv${uniqueString(resourceGroup().id)}', 24)

@description('Name of the frontend container app.')
param frontendAppName string = 'frontend-app'

@description('Name of the LLM Proxy container app.')
param llmProxyAppName string = 'llmproxy-app'

@description('Name of the AI Content container app.')
param aiContentAppName string = 'aicontent-app'

var tags = {
  app: 'k5-containercrew'
  environment: 'azure-container-apps'
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${environmentName}-logs'
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: environmentName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: containerRegistryName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: false
    enabledForTemplateDeployment: true
    accessPolicies: []
  }
}

resource geminiApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'gemini-api-key'
  properties: {
    value: 'AQ.Ab8RN6Kay7KuDIUX4-VxAQAGMMo19lZzZTIC_oNyOk-m0C3kGQ'
  }
}

resource serviceBApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'serviceb-api-key'
  properties: {
    value: 'super-secret-key-123'
  }
}

resource frontendIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${frontendAppName}-identity'
  location: location
  tags: tags
}

resource llmProxyIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${llmProxyAppName}-identity'
  location: location
  tags: tags
}

resource aiContentIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${aiContentAppName}-identity'
  location: location
  tags: tags
}

resource keyVaultAccessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: frontendIdentity.properties.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: llmProxyIdentity.properties.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: aiContentIdentity.properties.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}

resource frontendApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: frontendAppName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${frontendIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
      }
      secrets: []
    }
    template: {
      containers: [
        {
          name: 'frontend'
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
}

resource llmProxyApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: llmProxyAppName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${llmProxyIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: false
        targetPort: 8080
        transport: 'auto'
      }
      secrets: [
        {
          name: 'gemini-api-key'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/gemini-api-key)'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'llmproxy'
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Container'
            }
            {
              name: 'Gemini__ApiKey'
              secretRef: 'gemini-api-key'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 2
      }
    }
  }
}

resource aiContentApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: aiContentAppName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${aiContentIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
      }
      secrets: [
        {
          name: 'serviceb-api-key'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/serviceb-api-key)'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'aicontent'
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Container'
            }
            {
              name: 'LlmProxy__BaseUrl'
              value: 'https://${llmProxyAppName}.azurecontainerapps.io/'
            }
            {
              name: 'ServiceB__ApiKey'
              secretRef: 'serviceb-api-key'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
}

output containerRegistryLoginServer string = containerRegistry.properties.loginServer
output containerAppsEnvironmentName string = containerAppsEnvironment.name
output frontendUrl string = 'https://${frontendApp.properties.configuration.ingress.fqdn}'
output llmProxyUrl string = 'https://${llmProxyApp.properties.configuration.ingress.fqdn}'
output aiContentUrl string = 'https://${aiContentApp.properties.configuration.ingress.fqdn}'
