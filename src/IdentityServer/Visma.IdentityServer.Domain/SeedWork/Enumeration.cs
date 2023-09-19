using System.Reflection;

namespace Visma.IdentityServer.Domain.SeedWork;

/// <summary>
/// Enumeration base class.
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Initializes a new instance of a <see cref="Enumeration"/> class.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Enumeration()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Initializes a new instance of a <see cref="Enumeration"/> class.
    /// </summary>
    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Converts the current instance into its string representation.
    /// </summary>
    /// <returns>A <see cref="string"/> representation for the current enumeration.</returns>
    public override string ToString()
        => Name;

    /// <summary>
    /// Gets all of the values for the enumeration.
    /// </summary>
    /// <typeparam name="T">Type of enumeration.</typeparam>
    /// <returns>List of all possible values for the enumeration.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (var info in fields)
        {
            var instance = new T();

            if (info.GetValue(instance) is T locatedValue)
                yield return locatedValue;
        }
    }

    /// <summary>
    /// Determines if the input object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The input object.</param>
    /// <returns>True if equal and False if not.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    /// <summary>
    /// Gets the hash code for the current instance.
    /// </summary>
    /// <returns>Hash code as an <see cref="int"/>.</returns>
    public override int GetHashCode()
        => Id.GetHashCode();

    /// <summary>
    /// Obtains the absolute difference between two enumeration identifiers.
    /// </summary>
    /// <param name="firstValue">The first enumeration.</param>
    /// <param name="secondValue">The second enumeration.</param>
    /// <returns>The absolute difference.</returns>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    /// <summary>
    /// Obtains the enumeration value from its identifier.
    /// </summary>
    /// <typeparam name="T">Type of enumeration.</typeparam>
    /// <param name="value">The identifier.</param>
    /// <returns>The enumeration value.</returns>
    public static T FromValue<T>(int value) where T : Enumeration, new()
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    /// <summary>
    /// Obtains the enumeration value from its display name.
    /// </summary>
    /// <typeparam name="T">Type of enumeration.</typeparam>
    /// <param name="value">The display name.</param>
    /// <returns>The enumeration value.</returns>
    public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration, new()
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);
        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    /// <summary>
    /// Compares an object to a value.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The comparison result.</returns>
    public int CompareTo(object? obj)
        => obj is not null ? Id.CompareTo(((Enumeration)obj).Id) : -1;
}