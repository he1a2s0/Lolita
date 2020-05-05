using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    public interface ILolitaUpdateExecutor
    {
        string GenerateSql<TEntity>(LolitaSetting<TEntity> lolita) where TEntity : class;

        int Execute(DbContext db, string sql, object[] param);

        Task<int> ExecuteAsync(DbContext db, string sql, CancellationToken cancellationToken = default, params object[] param);
    }
}
