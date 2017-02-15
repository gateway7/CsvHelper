
#if !NET_2_0

#endif

namespace CsvHelper
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Extensions to help with reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the type from the property/field.
        /// </summary>
        /// <param name="member">The member to get the type from.</param>
        /// <returns>The type.</returns>
        public static Type MemberType(this MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            if (member is FieldInfo field)
            {
                return field.FieldType;
            }

            throw new InvalidOperationException("Member is not a property or a field.");
        }

#if !NET_2_0

        /// <summary>
        /// Gets a member expression for the property/field.
        /// </summary>
        /// <param name="member">The member to get the expression for.</param>
        /// <param name="expression">The member expression.</param>
        /// <returns>The member expression.</returns>
        public static MemberExpression GetMemberExpression(this MemberInfo member, Expression expression)
        {
            if (member is PropertyInfo property)
            {
                return Expression.Property(expression, property);
            }

            if (member is FieldInfo field)
            {
                return Expression.Field(expression, field);
            }

            throw new InvalidOperationException("Member is not a property or a field.");
        }

#endif
    }
}