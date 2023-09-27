namespace Example.IdentityServer.Domain.SeedWork;

/// <summary>
/// Base class for an entity.
/// </summary>
public abstract class Entity
{
    #region Fields

    private int? _requestedHashCode;

    #endregion

    #region Properties

    /// <summary>
    /// The identifier.
    /// </summary>
    public virtual Guid Id { get; private set; }

    /// <summary>
    /// The row version.
    /// </summary>
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    /// <summary>
    /// The date of creation.
    /// </summary>
    public DateTimeOffset CreationDate { get; private set; }

    /// <summary>
    /// The date of modification.
    /// </summary>
    public DateTimeOffset? ModificationDate { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether this instance is transient.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is transient; otherwise, <c>false</c>.
    /// </returns>
    public bool IsTransient()
        => Id == Guid.Empty;

    /// <summary>
    /// Determines if the input object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The input object.</param>
    /// <returns>True if equal and False if not.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity)
            return false;

        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var item = (Entity)obj;

        if (item.IsTransient() || IsTransient())
            return false;

        return item.Id == Id;
    }

    /// <summary>
    /// Gets the hash code for the current instance.
    /// </summary>
    /// <returns>Hash code as <see cref="int"/>.</returns>
    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                _requestedHashCode = Id.GetHashCode() ^ 31;
            }

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    #endregion
}