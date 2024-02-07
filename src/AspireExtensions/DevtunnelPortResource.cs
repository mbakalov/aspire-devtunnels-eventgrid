using Microsoft.DevTunnels.Contracts;
using Microsoft.DevTunnels.Management;
using System.Net.Http.Headers;

namespace AspireExtensions;

public class DevtunnelPortResource(string name, ushort port, DevTunnelResource parent) : Resource(name), IResourceWithParent<DevTunnelResource>
{
    public DevTunnelResource Parent => parent;

    public Uri? PublicUri { get; private set; }

    public async Task LoadPublicDevtunnelUriAsync()
    {
        // Use devtunnels sdk to get public url for this devtunnel/port

        var userAgent = new ProductInfoHeaderValue("TunnelSDKExperiments", null);
        var client = new TunnelManagementClient(userAgent, null, ManagementApiVersions.Version20230927Preview);

        var input = new Tunnel()
        {
            Name = Parent.Name
        };

        var tunnel = await client.GetTunnelAsync(input, null, CancellationToken.None);

        if (tunnel != null)
        {
            var tunnelPort = await client.GetTunnelPortAsync(tunnel, port, null, CancellationToken.None);

            if (tunnelPort != null)
            {
                var uri = tunnelPort.PortForwardingUris?.Single();

                if (uri != null)
                {
                    PublicUri = new Uri(uri);
                }
            }
        }
    }
}
