namespace Visma.IdentityServer.Application.Database;

/// <summary>
/// The database seeder for seeding the database with real data.
/// </summary>
internal interface IDatabaseSeeder
{
    /// <summary>
    /// Seed database with data.
    /// </summary>
    /// <returns>A task that completes when database has been seeded.</returns>
    Task SeedData();
}