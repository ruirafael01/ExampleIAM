using AutoFixture.Dsl;
using AutoMapper.Internal;
using System.Linq.Expressions;

namespace Visma.Common.UnitTests.Extensions;

/// <summary>
/// Test fixture extension methods.
/// </summary>
public static class TestFixtureExtensions
{
    /// <summary>
    /// Sets a specific value for a property.
    /// </summary>
    /// <typeparam name="T">The type T.</typeparam>
    /// <param name="composer">The composer.</param>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="value">The property value.</param>
    /// <returns>IPostprocessComposer</returns>
    public static IPostprocessComposer<T> WithCustomPropertyValue<T>(this IPostprocessComposer<T> composer, Expression<Func<T, object>> propertyExpression, object value)
    {
        var expression = propertyExpression.Body;
        if (expression is UnaryExpression unaryExpression)
        {
            expression = unaryExpression.Operand;
        }

        if (expression is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            return composer.Do(x => x.GetType().GetProperty(propertyName).SetMemberValue(x, value));
        }

        throw new ArgumentException("Invalid property expression.", nameof(propertyExpression));
    }
}