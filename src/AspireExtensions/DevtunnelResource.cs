namespace AspireExtensions;

public class DevTunnelResource(string name, string command, string workingDirectory, string[]? args)
    : ExecutableResource(name, command, workingDirectory, args), IResourceWithServiceDiscovery
{
}
