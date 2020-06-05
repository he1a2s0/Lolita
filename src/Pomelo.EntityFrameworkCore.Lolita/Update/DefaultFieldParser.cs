
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    public class DefaultFieldParser : IFieldParser
    {
        private static FieldInfo EntityTypesField = typeof(Model).GetTypeInfo().DeclaredFields.Single(x => x.Name == "_entityTypes");

        public DefaultFieldParser(ICurrentDbContext CurrentDbContext, ISqlGenerationHelper SqlGenerationHelper, IDbSetFinder DbSetFinder)
        {
            sqlGenerationHelper = SqlGenerationHelper;
            dbSetFinder = DbSetFinder;
            context = CurrentDbContext.Context;
        }

        private ISqlGenerationHelper sqlGenerationHelper;
        private IDbSetFinder dbSetFinder;
        private DbContext context;

        public virtual string ParseField(SqlFieldInfo field)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(field.Table))
                sb.Append(sqlGenerationHelper.DelimitIdentifier(field.Table))
                    .Append(".");
            sb.Append(sqlGenerationHelper.DelimitIdentifier(field.Column));
            return sb.ToString();
        }

        public virtual string ParseFullTable(SqlFieldInfo field)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(field.Database))
                sb.Append(sqlGenerationHelper.DelimitIdentifier(field.Database))
                    .Append(".");
            if (!string.IsNullOrEmpty(field.Schema))
                sb.Append(sqlGenerationHelper.DelimitIdentifier(field.Schema))
                    .Append(".");
            sb.Append(sqlGenerationHelper.DelimitIdentifier(field.Table));
            return sb.ToString();
        }

        public virtual string ParseShortTable(SqlFieldInfo field)
        {
            return sqlGenerationHelper.DelimitIdentifier(field.Table);
        }

        protected virtual string GetTableName(IEntityType type)
        {
            return type.GetTableName();
        }

        protected virtual string GetSchemaName(IEntityType type)
        {
            return type.GetSchema();
        }

        public virtual SqlFieldInfo VisitField<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> exp)
            where TEntity : class
        {
            var ret = new SqlFieldInfo();

            // Getting table name and schema name
            if (exp.Parameters.Count != 1)
            {
                throw new ArgumentException("Too many parameters in the expression.");
            }
            var param = exp.Parameters.Single();
            var et = context.Model.FindEntityType(typeof(TEntity));
            ret.Table = GetTableName(et);
            ret.Schema = GetSchemaName(et);


            // Getting field name
            var body = exp.Body as MemberExpression;
            if (body == null)
            {
                throw new NotSupportedException(exp.Body.GetType().Name);
            }

            var columnAttr = body.Member.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr != null)
            {
                ret.Column = columnAttr.Name;
            }
            else
            {
                ret.Column = et.FindProperty(body.Member.Name).GetColumnName();
            }

            return ret;
        }
    }
}
