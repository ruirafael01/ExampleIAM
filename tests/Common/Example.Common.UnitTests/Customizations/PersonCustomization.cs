using AutoFixture;
using Bogus;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Person = Example.IdentityServer.Domain.AggregatesModel.PersonAggregate.Person;

namespace Example.Common.UnitTests.Customizations;

/// <summary>
/// Customization for a <see cref="Person"/>.
/// </summary>
public sealed class PersonCustomization : ICustomization
{
    /// <summary>
    /// Customize the generation of a person.
    /// </summary>
    /// <param name="fixture">The test fixture.</param>
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => GeneratePerson(fixture));
    }

    private static Person GeneratePerson(IFixture fixture)
        => new Faker<Person>()
            .CustomInstantiator(f => Create(fixture))
            .RuleFor(f => f.Id, Guid.NewGuid())
            .RuleFor(f => f.RowVersion, fixture.CreateMany<byte>().ToArray())
            .Generate();

    private static Person Create(IFixture fixture)
         => new(fixture.Create<PersonPassword>(),
                fixture.Create<PersonName>(),
                fixture.Create<PersonRole>(),
                fixture.Create<PersonLanguage>(),
                fixture.Create<PersonEmail>());
}