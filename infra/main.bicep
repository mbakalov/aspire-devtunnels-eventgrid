targetScope = 'subscription'

@description('Name of the resource group')
param resourceGroupName string

param location string = 'eastus2'

var nameSuffix = toLower(uniqueString(subscription().id, resourceGroupName))
var communictionServiceName = 'acs-${nameSuffix}'

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module communicationService 'communicationservices.bicep' = {
  name: 'communicationService'
  scope: resourceGroup
  params: {
    communicationServiceName: communictionServiceName
  }
}

output ACS_CONNECTIONSTRING string = communicationService.outputs.primaryConnectionString
output ACS_ENDPOINT string = communicationService.outputs.endpoint
output ACS_SERVICE_NAME string = communicationService.outputs.communicationServiceName
output ACS_RESOURCE_ARM_ID string = communicationService.outputs.id
