using WongaLoginService.Tests.Fixtures;
using Xunit;

namespace WongaLoginService.Tests;

/// <summary>
/// Defines a shared collection fixture for integration tests.
/// Uses ICollectionFixture to create a single PostgreSQL container shared across all integration tests.
/// This prevents ResourceReaper conflicts that occur when multiple containers are initialized simultaneously.
/// </summary>
[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    // Will apply [CollectionDefinition] and ICollectionFixture<>
}
