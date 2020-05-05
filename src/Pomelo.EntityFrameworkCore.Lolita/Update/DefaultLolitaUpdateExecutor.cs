
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    public class DefaultLolitaUpdateExecutor : ILolitaUpdateExecutor
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;

        public DefaultLolitaUpdateExecutor(ISqlGenerationHelper SqlGenerationHelper)
        {
            _sqlGenerationHelper = SqlGenerationHelper;
        }

        public virtual string GenerateSql<TEntity>(LolitaSetting<TEntity> lolita) where TEntity : class
        {
            var originalSql = lolita.Query.ToSql(out IDictionary<string, string> tableAliases);

            var sb = new StringBuilder("UPDATE ");
            sb.Append(lolita.FullTable)
                .AppendLine()
                .Append("SET ")
                .Append(string.Join($", { Environment.NewLine }    ", lolita.Operations))
                .AppendLine()
                .Append(ParseWhere(originalSql, tableAliases))
                .Append(_sqlGenerationHelper.StatementTerminator);

            return sb.ToString();
        }

        protected virtual string ParseWhere(string sql, IDictionary<string, string> tableAliases)
        {
            var pos = sql.IndexOf("WHERE");
            if (pos < 0)
                return "";
            sql = sql.Substring(pos);

            foreach (var pair in tableAliases)
            {
                sql = sql.Replace(_sqlGenerationHelper.DelimitIdentifier(pair.Key), _sqlGenerationHelper.DelimitIdentifier(pair.Value));
            }

            return sql;
        }

        public virtual int Execute(DbContext db, string sql, object[] param)
        {
            return db.Database.ExecuteSqlRaw(sql, param);
        }

        public Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default, params object[] param)
        {
            return db.Database.ExecuteSqlRawAsync(sql, param, cancellationToken);
        }
    }
}
