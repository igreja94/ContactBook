using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Webhost.Tests.Fixtures;

// Runs the real AppHost with SqlServer:Ephemeral=true so a brand new SQL Server container is started
// (random name, no data volume, removed on teardown) instead of reusing the developer's persistent one.
// The Webhost migrates and seeds the database on startup.
public sealed class WebhostFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(
            ["--SqlServer:Ephemeral=true"], cts.Token);

        // The ASP.NET Core dev certificate is not trusted on CI agents, so accept it for the test clients.
        builder.Services.ConfigureHttpClientDefaults(http => http.ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }));

        _app = await builder.BuildAsync(cts.Token);
        await _app.StartAsync(cts.Token);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("webhost", cts.Token);

        Client = _app.CreateHttpClient("webhost");
        await WaitUntilReadyAsync(cts.Token);
    }

    // The Webhost only starts listening once the database is migrated and seeded.
    private async Task WaitUntilReadyAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                using var response = await Client.GetAsync("/health", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }

            await Task.Delay(500, cancellationToken);
        }
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}

[CollectionDefinition(Name)]
public sealed class WebhostCollection : ICollectionFixture<WebhostFixture>
{
    public const string Name = "Webhost";
}
