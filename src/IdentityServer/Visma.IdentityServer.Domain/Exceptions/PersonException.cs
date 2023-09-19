using System.Runtime.Serialization;

namespace Visma.IdentityServer.Domain.Exceptions;

/// <summary>
/// <see cref="Person"/> domain exception.
/// </summary>
[Serializable]
public sealed class PersonException : Exception
{
    /// <summary>
    /// Empty initialization of  <see cref="PersonException"/>.
    /// </summary>
    public PersonException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonException"/> exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PersonException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonException"/> exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PersonException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Constructor for ISerializable
    private PersonException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
