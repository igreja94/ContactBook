namespace Webhost.Tests.Fixtures;

[CollectionDefinition(Name)]
public sealed class WebhostCollectionDefinition : ICollectionFixture<WebhostFixture>
{
    public const string Name = "Webhost";
}
