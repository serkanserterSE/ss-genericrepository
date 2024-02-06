using com.ss.genericrepository.concretes;
using Microsoft.EntityFrameworkCore;

namespace com.ss.genericrepository.interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext GetDbContext();
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        UnitOfWork SetCurrentUserId(Guid userId);
    }
}