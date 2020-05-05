
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// see https://gist.github.com/rionmonster/2c59f449e67edf8cd6164e9fe66c545a#gistcomment-3040767
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string ToSql<TEntity>(this IQueryable<TEntity> query, out IDictionary<string, string> aliases)
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var enumeratorType = enumerator.GetType();

            var selectFieldInfo = enumeratorType.GetField("_selectExpression", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException($"cannot find field _selectExpression on type {enumeratorType.Name}");

            var sqlGeneratorFieldInfo = enumeratorType.GetField("_querySqlGeneratorFactory", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException($"cannot find field _querySqlGeneratorFactory on type {enumeratorType.Name}");

            var selectExpression = selectFieldInfo.GetValue(enumerator) as SelectExpression ?? throw new InvalidOperationException($"could not get SelectExpression");

            var dbcontext = query.GetDbContext();
            var entityType = dbcontext.Model.FindEntityType(typeof(TEntity));

            aliases = selectExpression.Tables.Cast<TableExpression>().ToDictionary(_ => _.Alias, _ => _.Name);

            var factory = sqlGeneratorFieldInfo.GetValue(enumerator) as IQuerySqlGeneratorFactory ?? throw new InvalidOperationException($"could not get IQuerySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            var sql = command.CommandText;

            return sql;
        }
    }
}
