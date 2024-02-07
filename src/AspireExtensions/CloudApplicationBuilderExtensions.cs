namespace AspireExtensions;

public static class CloudApplicationBuilderExtensions
{
    public static IResourceBuilder<AzureCommunicationServicesResource> AddAzureCommunicationServices(
        this IDistributedApplicationBuilder builder,
        string name,
        string? connectionString = null)
    {
        var connection = new AzureCommunicationServicesResource(name, connectionString);
        return builder.AddResource(connection);
    }
}
