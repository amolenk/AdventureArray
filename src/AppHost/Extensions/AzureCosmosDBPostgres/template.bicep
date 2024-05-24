targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
@secure()
param cosmosDbPassword string

resource cosmosDbPostgreSql 'Microsoft.DBforPostgreSQL/serverGroupsv2@2022-11-08' = {
  name: toLower(take('cosmosdb-postgres${uniqueString(resourceGroup().id)}', 24))
  location: location
  properties: {
    administratorLoginPassword: cosmosDbPassword
    coordinatorEnablePublicIpAccess: true
    coordinatorServerEdition: 'GeneralPurpose'
    coordinatorStorageQuotaInMb: 131072 // 128 * 1024
    coordinatorVCores: 4
    nodeCount: 2
    nodeEnablePublicIpAccess: true
    nodeServerEdition: 'MemoryOptimized'
    nodeStorageQuotaInMb: 1048576 // 1024 * 1024
    nodeVCores: 4
  }

  resource allowAzureServiceRule 'firewallRules' = {
    name: 'allow-azure-services'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
}

output connectionString string = 'Host=${cosmosDbPostgreSql.properties.serverNames[0].fullyQualifiedDomainName}:5432;Database=citus;Username=citus;Password=${cosmosDbPassword}'

