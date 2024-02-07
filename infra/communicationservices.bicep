param communicationServiceName string

resource communicationService 'Microsoft.Communication/CommunicationServices@2023-04-01-preview' = {
  name: communicationServiceName
  location: 'global'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dataLocation: 'united states'
  }
}

output endpoint string = 'https://${communicationService.properties.hostName}'
#disable-next-line outputs-should-not-contain-secrets
output primaryConnectionString string = communicationService.listKeys().primaryConnectionString
output communicationServiceName string = communicationServiceName
output id string = communicationService.id
