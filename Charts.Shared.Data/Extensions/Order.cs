using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Charts.Shared.Data.Extensions
{
    public static class Order
    {
        public static IOrderedQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> query, string propertyName, string fallbackProperty, string direction = "asc")
        {
            var entityType = typeof(TSource);

            var propertyInfo = propertyName == null ? null : entityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                propertyName = fallbackProperty;
                propertyInfo = entityType.GetProperty(propertyName);
            }


            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            var enumarableType = typeof(Queryable);
            var method = enumarableType.GetMethods()
                    .Where(m => m.Name == (direction == "asc" ? "OrderBy" : "OrderByDescending") && m.IsGenericMethodDefinition)
                    .Where(m =>
                    {
                        var parameters = m.GetParameters().ToList();
                        return parameters.Count == 2;
                    }).Single();

            MethodInfo genericMethod = method
                    .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                    .Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }
    }
}