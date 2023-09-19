using AutoFixture;
using AutoFixture.Dsl;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Visma.Common.UnitTests.Customizations;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Visma.Common.UnitTests.Fixtures;

/// <summary>
/// Fixture having factories for various types registered.
/// </summary>
public class TestFixture
{
    private readonly Fixture _fixture;

    /// <summary>
    /// Initializes a new instance of a <see cref="TestFixture"/> class.
    /// </summary>
    public TestFixture()
    {
        _fixture = new Fixture();

        _fixture.Register(GeneratePersonName);
        _fixture.Register(GeneratePersonEmail);
        _fixture.Register(GeneratePersonPassword);
        _fixture.Register(GenerateStream);
        _fixture.Register(GenerateClaim);
        _fixture.Register(GeneratePathString);

        _fixture.Customize(new PersonCustomization());

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    #region Methods

    /// <summary>
    /// The internal fixture.
    /// </summary>
    public Fixture Fixture => _fixture;

    /// <summary>
    /// Registers a factory for the given type T.
    /// </summary>
    /// <typeparam name="T">The type to produce.</typeparam>
    /// <param name="factory">The factory callback.</param>
    public void Register<T>(Func<T> factory) => _fixture.Register(factory);

    /// <summary>
    /// Creates an anonymous instance of the given type T.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <returns>A new instance of given type.</returns>
    public T Create<T>()
        => _fixture.Create<T>();

    /// <summary>
    /// Creates an enumeration of instances of type T.
    /// </summary>
    /// <typeparam name="T">The requested type to produce.</typeparam>
    /// <param name="numInstances">The number of instances requested.</param>
    /// <returns>An enumeration of instances.</returns>
    public IEnumerable<T> CreateEnumeration<T>(int numInstances = 2)
        => _fixture.CreateMany<T>(numInstances);

    /// <summary>
    /// Creates a list of instances of type T.
    /// </summary>
    /// <typeparam name="T">The requested type to produce.</typeparam>
    /// <param name="numInstances">The number of instances requested.</param>
    /// <returns>A new instance of a <see cref="List{T}"/>.</returns>
    public List<T> CreateList<T>(int numInstances = 2)
        => CreateEnumeration<T>(numInstances).ToList();

    /// <summary>
    /// Creates an array of instances of type T.
    /// </summary>
    /// <typeparam name="T">The requested type to produce.</typeparam>
    /// <param name="numInstances">The number of instances requested.</param>
    /// <returns>A new instance of a <see cref="{T}[]"/>.</returns>
    public T[] CreateArray<T>(int numInstances = 2)
        => CreateEnumeration<T>(numInstances).ToArray();

    /// <summary>
    /// Injects a specific instance for a specific type, in order to make that instance
    /// a shared instance, no matter how many times the fixture is asked to create an
    /// instance of that type.
    /// </summary>
    /// <typeparam name="T">The type to inject.</typeparam>
    /// <param name="value">The value to inject.</param>
    public void Inject<T>(T value)
        => _fixture.Inject(value);

    /// <summary>
    /// Customizes the creation algorithm for a single object, effectively turning off
    /// all Customizations on the AutoFixture.IFixture.
    /// </summary>
    /// <typeparam name="T">The type to customize.</typeparam>
    public ICustomizationComposer<T> Build<T>()
        => _fixture.Build<T>();

    #endregion


    private static PersonName GeneratePersonName()
    {
        var generator = new Bogus.Person();
        return new(generator.FirstName, generator.LastName);
    }

    private static PersonEmail GeneratePersonEmail()
    {
        var generator = new Bogus.Person();
        return new(generator.Email);
    }

    private PersonPassword GeneratePersonPassword()
    {
        var randomNumbers = string.Join("", CreateEnumeration<int>(6)).Substring(0, 6);

        var lowerCaseLetters = new string(CreateEnumeration<char>(20)
            .Where(c => char.IsLetter(c)).ToArray()).ToLowerInvariant();

        var upperCaseLetters = new string(CreateEnumeration<char>(20)
            .Where(c => char.IsLetter(c)).ToArray()).ToUpperInvariant();

        var specialCharacter = "!";

        return new($"{randomNumbers}{lowerCaseLetters}{upperCaseLetters}{specialCharacter}");
    }

    private Stream GenerateStream()
        => new MemoryStream(CreateEnumeration<byte>(100).ToArray());

    private Claim GenerateClaim()
        => new Claim(Create<string>(), Create<string>());

    private PathString GeneratePathString()
        => new PathString($"/{Create<string>()}");
}