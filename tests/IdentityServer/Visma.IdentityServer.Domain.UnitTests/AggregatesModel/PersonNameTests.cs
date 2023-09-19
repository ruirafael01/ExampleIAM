﻿using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.Exceptions;

namespace Visma.IdentityServer.Domain.UnitTests.AggregatesModel;

/// <summary>
/// Unit tests for the <see cref="PersonName"/>.
/// </summary>
public sealed class PersonNameTests
{
    [Theory]
    [InlineData(null, "lastName")]
    [InlineData("firstName", null)]
    [InlineData("", "lastName")]
    [InlineData("firstName", "")]
    public void Constructor_WithInvalidName_ThrowsDomainException(string firstName, string lastName)
    {
        // Act & Assert
        Assert.Throws<PersonException>(() => new PersonName(firstName, lastName));
    }
}