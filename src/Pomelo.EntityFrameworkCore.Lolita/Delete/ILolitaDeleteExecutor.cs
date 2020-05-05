
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pomelo.EntityFrameworkCore.Lolita.Delete
{
    public interface ILolitaDeleteExecutor
    {
        string GenerateSql<TEntity>(IQueryable<TEntity> lolita) where TEntity : class;

        int Execute(DbContext db, string sql);

        Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default(CancellationToken));
    }
}
