using System.Linq.Expressions;
using com.ss.genericrepository.models;

namespace com.ss.genericrepository.interfaces
{
    public interface IGenericRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void SoftDelete(TEntity entity);
        void HardDelete(TEntity entity);
        TEntity? GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        TEntity? GetLastOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null);
        Task<TEntity?> GetLastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null);
        Task<Pagination<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        uint pageNumber = 0,
        uint pageSize = 10);
    }
}

