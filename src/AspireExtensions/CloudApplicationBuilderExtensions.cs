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

    public static IResourceBuilder<DevTunnelResource> AddDevTunnel(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        string[] allArgs = ["host", name];

        var resource = new DevTunnelResource(name, "devtunnel", builder.AppHostDirectory, allArgs);

        return builder.AddResource(resource);
    }

    public static IResourceBuilder<DevtunnelPortResource> AddPort(
        this IResourceBuilder<DevTunnelResource> builder,
        string name,
        ushort port)
    {
        var portResource = new DevtunnelPortResource(name, port, builder.Resource);
        return builder.ApplicationBuilder.AddResource(portResource);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<DevtunnelPortResource> source)
        where TDestination: IResourceWithEnvironment
    {
        var portResource = source.Resource;

        if (portResource.PublicUri == null)
        {
            throw new ApplicationException("PublicUri on the DevtunnelPortResource must be initialized by calling LoadPublicDevtunnelUriAsync");
        }

        return builder.WithEnvironment(context =>
        {
            // TODO: is there an aspnet convention for "my hostname on <some network>"?
            context.EnvironmentVariables["EndpointUrl"] = portResource.PublicUri.ToString();
        });
    }
}
