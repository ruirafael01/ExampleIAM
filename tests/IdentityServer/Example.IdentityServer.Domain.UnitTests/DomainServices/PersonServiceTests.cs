using Example.Common.UnitTests.Fixtures;
using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Example.IdentityServer.Domain.DomainOperations;
using Example.IdentityServer.Domain.DomainServices;
using Example.IdentityServer.Domain.Models;
using Example.IdentityServer.Domain.SeedWork;

namespace Example.IdentityServer.Domain.UnitTests.DomainServices;

/// <summary>
/// Unit tests for the <see cref="PersonService"/>.
/// </summary>
public sealed class PersonServiceTests : TestFixture
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IPersonRepository> _mockPersonRepository;
    private readonly Mock<ICryptographer> _mockCryptographer;
    private readonly IPersonService _personService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonServiceTests"/>
    /// </summary>
    public PersonServiceTests()
    {
        _mockUow = new();
        _mockPersonRepository = new();
        _mockCryptographer = new();

        _personService = new PersonService(_mockPersonRepository.Object, _mockCryptographer.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PersonService(null, _mockCryptographer.Object));
    }

    [Fact]
    public void Constructor_WithNullCryptographer_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PersonService(_mockPersonRepository.Object, null));
    }

    [Fact]
    public async Task CreatePersonAsync_WithValidArguments_CreatesPerson()
    {
        // Arrange
        var person = Create<Person>();

        _mockCryptographer.Setup(x => x.Hash(person.Password.Value))
            .Returns(new string(CreateEnumeration<char>(100).ToArray()));

        _mockPersonRepository.Setup(x => x.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        _mockUow
            .Setup(x => x.SaveEntitiesAsync(CancellationToken.None))
            .ReturnsAsync(DomainResponse.Success);

        _mockPersonRepository
            .Setup(x => x.UnitOfWork)
            .Returns(_mockUow.Object);

        // Act
        await _personService.CreatePersonAsync(person.Password.Value,
                                               person.Email,
                                               person.Language,
                                               person.Role,
                                               person.Name,
                                               It.IsAny<CancellationToken>());

        // Assert
        _mockCryptographer.Verify(x => x.Hash(person.Password.Value), Times.Exactly(1));

        _mockPersonRepository.Verify(x => x.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

        _mockUow.Verify(x => x.SaveEntitiesAsync(CancellationToken.None), Times.Exactly(1));

        _mockPersonRepository.Verify(x => x.UnitOfWork, Times.Exactly(1));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PasswordIsEqual_WithValidInputStrings_ReturnsExpectedResult(bool expected)
    {
        // Arrange
        var person = Create<Person>();
        var inputPassword = Create<string>();

        _mockCryptographer.Setup(x => x.ValidateHash(person.Password.Value, inputPassword))
            .Returns(expected);

        // Act
        var response = _personService.PasswordIsEqual(person, inputPassword);

        // Assert
        response.Should().Be(expected);

        _mockCryptographer.Verify(x => x.ValidateHash(person.Password.Value, inputPassword), Times.Exactly(1));
    }
}
