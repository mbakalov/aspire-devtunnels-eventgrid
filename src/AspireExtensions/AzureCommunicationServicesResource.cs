namespace AspireExtensions;

public class AzureCommunicationServicesResource(string name, string? connectionString)
    : Resource(name), IResourceWithConnectionString, IAzureResource
{
    public string? ConnectionString { get; set; } = connectionString;

    public string? GetConnectionString() => ConnectionString;
}
