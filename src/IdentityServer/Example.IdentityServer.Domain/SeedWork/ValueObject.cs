namespace Example.IdentityServer.Domain.SeedWork;

/// <summary>
/// Value object base class.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// List of all the atomic values for the value object.
    /// </summary>
    /// <returns>List of atomic values</returns>
    protected abstract IEnumerable<object> GetAtomicValues();

    /// <summary>
    /// Equals operator.
    /// </summary>
    /// <param name="obj">Object to be compared to.</param>
    /// <returns>True if equal and False if not equal.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        var thisValues = GetAtomicValues().GetEnumerator();
        var otherValues = other.GetAtomicValues().GetEnumerator();
        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                return false;

            if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                return false;
        }
        return !thisValues.MoveNext() && !otherValues.MoveNext();
    }

    /// <summary>
    /// Gets the hash code of the value object.
    /// </summary>
    /// <returns>Hash code as <see cref="int"/>.</returns>
    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Gets a copy for the current object.
    /// </summary>
    /// <returns>Copy of the value object.</returns>
    public ValueObject GetCopy()
        => (ValueObject)MemberwiseClone();
}