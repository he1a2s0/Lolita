
using System.Linq;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.Metadata;

using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.Lolita
{
    public static class IQueryableExtensions
    {
        public static IEntityType GetRealEntityType<TEntity>(this IQueryable<TEntity> self)
        {
            var genericEntityType = typeof(TEntity);

            if (genericEntityType.IsInterface || genericEntityType.IsAbstract)
            {
                var rootExpr = GetRootExpression(self);
                if (rootExpr == null) return null;

                var entityType = rootExpr.EntityType;

                return entityType;
            }

            return null;
        }

        private static QueryRootExpression GetRootExpression<TEntity>(IQueryable<TEntity> self)
        {
            var expr = self.Expression;

            while (expr is not QueryRootExpression)
            {
                var arguments = ((MethodCallExpression)expr).Arguments;

                expr = arguments.FirstOrDefault();
                if (expr == null) return null; ;
            }

            return expr as QueryRootExpression;
        }
    }
}
