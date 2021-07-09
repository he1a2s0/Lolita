
using System;
using System.Linq;
using System.Linq.Expressions;

using Pomelo.EntityFrameworkCore.Lolita;
using Pomelo.EntityFrameworkCore.Lolita.Update;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbSetExtensions
    {
        public static LolitaValuing<TEntity, TProperty> SetField<TEntity, TProperty>(this IQueryable<TEntity> self, Expression<Func<TEntity, TProperty>> SetValueExpression)
            where TEntity : class
        {
            if (SetValueExpression == null)
                throw new ArgumentNullException("SetValueExpression");

            var factory = self.GetService<IFieldParser>();
            var realEntityType = self.GetRealEntityType();

            var sqlfield =
                realEntityType == null ?
                factory.VisitField(SetValueExpression) :
                factory.VisitField(SetValueExpression, realEntityType);

            var inner = new LolitaSetting<TEntity> { Query = self, FullTable = factory.ParseFullTable(sqlfield), ShortTable = factory.ParseShortTable(sqlfield) };
            return new LolitaValuing<TEntity, TProperty> { Inner = inner, CurrentField = factory.ParseField(sqlfield) };
        }

        public static LolitaValuing<TEntity, TProperty> SetField<TEntity, TProperty>(this LolitaSetting<TEntity> self, Expression<Func<TEntity, TProperty>> SetValueExpression)
            where TEntity : class
        {
            if (SetValueExpression == null)
                throw new ArgumentNullException("SetValueExpression");

            var factory = self.GetService<IFieldParser>();
            var realEntityType = self.Query.GetRealEntityType();

            var sqlfield =
                            realEntityType == null ?
                            factory.VisitField(SetValueExpression) :
                            factory.VisitField(SetValueExpression, realEntityType);

            return new LolitaValuing<TEntity, TProperty> { Inner = self, CurrentField = factory.ParseField(sqlfield) };
        }
    }
}
