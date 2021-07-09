using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Metadata;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    public interface IFieldParser
    {
        SqlFieldInfo VisitField<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> exp) where TEntity: class;

        SqlFieldInfo VisitField<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> exp, IEntityType explicitEntityType) where TEntity : class;


        string ParseField(SqlFieldInfo field);

        string ParseFullTable(SqlFieldInfo field);

        string ParseShortTable(SqlFieldInfo field);
    }
}
