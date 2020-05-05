
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using Pomelo.EntityFrameworkCore.Lolita.Delete;
using Pomelo.EntityFrameworkCore.Lolita.Update;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.EntityFrameworkCore
{
    public class LolitaDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public LolitaDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        {
        }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "Pomelo.EFCore.Lolita";

        public override long GetServiceProviderHashCode()
        {
            return 86216188623901;
        }

        public override void PopulateDebugInfo([NotNull] IDictionary<string, string> debugInfo)
        {
        }
    }

    public class LolitaDbOptionExtension : IDbContextOptionsExtension
    {
        public DbContextOptionsExtensionInfo Info => new LolitaDbContextOptionsExtensionInfo(this);

        public void ApplyServices(IServiceCollection services)
        {
            services
                .AddScoped<IFieldParser, DefaultFieldParser>()
                .AddScoped<ISetFieldSqlGenerator, DefaultSetFieldSqlGenerator>()
                .AddScoped<ILolitaUpdateExecutor, DefaultLolitaUpdateExecutor>()
                .AddScoped<ILolitaDeleteExecutor, DefaultLolitaDeleteExecutor>();
        }


        public void Validate(IDbContextOptions options)
        {
        }
    }

    public static class LolitaDbOptionExtensions
    {
        public static DbContextOptionsBuilder UseLolita(this DbContextOptionsBuilder self)
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseLolita<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }

        public static DbContextOptions UseLolita(this DbContextOptions self)
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }

        public static DbContextOptions<TContext> UseLolita<TContext>(this DbContextOptions<TContext> self) where TContext : DbContext
        {
            ((IDbContextOptionsBuilderInfrastructure)self).AddOrUpdateExtension(new LolitaDbOptionExtension());
            return self;
        }
    }
}