
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

using System.Linq;
using System.Reflection;

namespace Pomelo.EntityFrameworkCore.Lolita
{
    internal static class ReflectionCommon
    {
        public static readonly FieldInfo QueryCompilerOfEntityQueryProvider = typeof(EntityQueryProvider).GetField("_queryCompiler", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

        public static readonly FieldInfo DatabaseOfQueryCompiler = typeof(QueryCompiler).GetField("_database", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

        public static readonly PropertyInfo DependenciesOfDatabase = typeof(Database).GetProperty("Dependencies", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic);

        public static readonly FieldInfo DependenciesOfQueryCompilerContextFactory = typeof(QueryCompilationContextFactory).GetField("_dependencies", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);


        public static DbContext GetCurrentDbContextByQueryable(IQueryable queryable)
        {
            if (queryable.GetType().GetGenericTypeDefinition() == typeof(EntityQueryable<>))
            {
                var queryCompiler = (QueryCompiler)QueryCompilerOfEntityQueryProvider.GetValue(queryable.Provider);
                var database = (RelationalDatabase)DatabaseOfQueryCompiler.GetValue(queryCompiler);
                var dbDependencies = (DatabaseDependencies)DependenciesOfDatabase.GetValue(database);
                var qccf = dbDependencies.QueryCompilationContextFactory;
                var qccfDependencies = (QueryCompilationContextDependencies)DependenciesOfQueryCompilerContextFactory.GetValue(qccf);
                return qccfDependencies.CurrentContext.Context;
            }
            else if (queryable.GetType().GetGenericTypeDefinition() == typeof(InternalDbSet<>))
            {
                var context = (DbContext)queryable.GetType().GetField("_context", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(queryable);
                return context;
            }

            return null;
        }
    }
}
