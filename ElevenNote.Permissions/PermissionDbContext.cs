using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ElevenNote.Permissions
{
    public interface IPermissionDbContext
    {
        DbSet<Permission> Permissions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}