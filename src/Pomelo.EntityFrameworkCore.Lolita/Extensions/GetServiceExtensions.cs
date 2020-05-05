
using Microsoft.EntityFrameworkCore.Infrastructure;

using Pomelo.EntityFrameworkCore.Lolita;

using System;
using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
    internal static class GetServiceExtensions
    {
        public static TService GetService<TService>(this IQueryable self)
        {
            var context = self.GetDbContext() ?? throw new NotSupportedException(self.GetType().Name);

            return context.GetService<TService>();
        }

        public static DbContext GetDbContext(this IQueryable self)
            => ReflectionCommon.GetCurrentDbContextByQueryable(self);
    }
}
