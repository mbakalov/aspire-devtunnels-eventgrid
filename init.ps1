function New-Devtunnel
{
    Write-Output "Creating aspire-tunnel..."

    $devtunelLogin = $(devtunnel user show)

    if ($devtunelLogin -eq "Not logged in.")
    {
        Write-Output "Logging in to devtunnel..."

        devtunnel login
    }

    $aspireTunnel = $(devtunnel list -l aspire-tunnel)

    if ($aspireTunnel -eq "No tunnels found.")
    {
        Write-Output "Creating aspire-tunnel"
        devtunnel create aspire-tunnel --allow-anonymous --labels aspire-tunnel

        Write-Output "Creating port 5099 for aspire-tunnel"
        devtunnel port create aspire-tunnel -p 5099 --protocol http

        Write-Output "Done"
    }
    else
    {
        Write-Output "aspire-tunnel already exists"
    }
}

function New-AzureCommunicationServicesResource
{
    Write-Output "Creating Communication Services resource in Azure..."

    $azAccounts = az account list | ConvertFrom-Json

    if ($azAccounts -eq $null)
    {
        Write-Output "az account list returned no subscriptions, trying to log in..."

        az login

        Write-Output "Logged in to Azure"
    }

    $azAccounts = az account list | ConvertFrom-Json

    Write-Output "Found $($azAccounts.Count) subscriptions"

    $subscriptionName = Read-Host "Enter name of the Azure subscription to deploy resources to"

    az account set --subscription $subscriptionName
    $subscriptionId = (az account show | ConvertFrom-Json).id

    Write-Output "Using subscriptionId: $subscriptionId"

    $rg = "rg-aspire-devtunnels-test"

    $templateFile=".\infra\main.bicep"

    $stamp = (Get-Date).ToString("yyyy-MM-dd-hh-mm")
    $deploymentName="deployment-$stamp"

    $deployResult = az deployment sub create `
        --name $deploymentName `
        --location eastus2 `
        --template-file $templateFile `
        --parameters resourceGroupName=$rg `
        --verbose | ConvertFrom-Json

    $ErrorActionPreference='Stop'
    if ($LASTEXITCODE)
    { 
        Write-Error "Error during az deployment, could not create resources"
        Throw "Error creating Azure resources"
    } 
        
    $outputs = $deployResult.properties.outputs

    Write-Output "Created Communication Services resource: $($outputs.ACS_SERVICE_NAME.Value)"
    
    Write-Output "Setting environment variables for the created Azure resources"
    [Environment]::SetEnvironmentVariable('ACS_SERVICE_NAME', $outputs.ACS_SERVICE_NAME.Value)
    [Environment]::SetEnvironmentVariable('ACS_RESOURCE_ARM_ID', $outputs.ACS_RESOURCE_ARM_ID.Value)
    
    # Todo: use dotnet user-secrets
    [Environment]::SetEnvironmentVariable('ConnectionStrings__ACS', $outputs.ACS_CONNECTIONSTRING.Value)

    Write-Output "Done"
}

New-Devtunnel
New-AzureCommunicationServicesResource
