
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class IQueryableExtensions
    {
        public static string ToSql<TEntity>(this IQueryable<TEntity> query, out IDictionary<string, string> aliases) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache") as RelationalCommandCache;
            var relationalQueryContext = enumerator.Private("_relationalQueryContext") as RelationalQueryContext;
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();

            aliases = selectExpression.Tables.Cast<TableExpression>().ToDictionary(_ => _.Alias, _ => _.Name);

            var command = sqlGenerator.GetCommand(selectExpression);
            var commandBuilder = sqlGenerator.GetType().GetProperty("Sql", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sqlGenerator) as IRelationalCommandBuilder;
            var mappingSource = commandBuilder.TypeMappingSource;

            string sql = command.CommandText;
            var parameterValues = relationalQueryContext.ParameterValues;

            if (parameterValues.Any())
            {
                foreach (var pair in parameterValues)
                {
                    var param = "@" + pair.Key;
                    if (sql.IndexOf(param) == -1)
                        continue;

                    sql = sql.Replace(param, mappingSource.GetMappingForValue(pair.Value).GenerateSqlLiteral(pair.Value));
                }
            }

            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}
