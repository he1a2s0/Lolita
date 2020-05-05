
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pomelo.EntityFrameworkCore.Lolita.Delete
{
    public class DefaultLolitaDeleteExecutor : ILolitaDeleteExecutor
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly DbContext _context;

        public DefaultLolitaDeleteExecutor(ICurrentDbContext CurrentDbContext, ISqlGenerationHelper SqlGenerationHelper)
        {
            _sqlGenerationHelper = SqlGenerationHelper;
            _context = CurrentDbContext.Context;
        }

        protected virtual string ParseTableName(IEntityType type) => type.GetTableName();

        protected virtual string GetTableName(IEntityType et)
        {
            return _sqlGenerationHelper.DelimitIdentifier(ParseTableName(et));
        }

        protected virtual string GetFullTableName(IEntityType et)
        {
            var schema = et.GetSchema() ?? et.GetDefaultSchema();

            if (schema != null)
                return $"{_sqlGenerationHelper.DelimitIdentifier(schema)}.{_sqlGenerationHelper.DelimitIdentifier(ParseTableName(et))}";
            else
                return _sqlGenerationHelper.DelimitIdentifier(ParseTableName(et));
        }

        public virtual string GenerateSql<TEntity>(IQueryable<TEntity> lolita) where TEntity : class
        {
            var et = _context.Model.FindEntityType(typeof(TEntity));

            var fullTable = GetFullTableName(et);

            var originalSql = lolita.ToSql(out IDictionary<string, string> tableAliases);

            var sb = new StringBuilder("DELETE FROM ");

            sb.Append(fullTable)
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

        public virtual int Execute(DbContext db, string sql)
        {
            return db.Database.ExecuteSqlRaw(sql);
        }

        public Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default)
        {
            return db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
    }
}
