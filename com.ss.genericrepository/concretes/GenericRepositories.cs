using System.Collections;
using System.Linq.Expressions;
using com.ss.genericrepository.interfaces;
using com.ss.genericrepository.models;
using Microsoft.EntityFrameworkCore;

namespace com.ss.genericrepository.concretes
{
    public class GenericRepository<TEntity>: IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<TEntity>();
        }

        private BaseEntity CastToBaseEntity(TEntity entity)
        {
            if (typeof(TEntity).IsSubclassOf(typeof(BaseEntity)))
                return entity as BaseEntity;
            else
                throw new ArgumentException("Entity is not valid!");
        }

        public void SoftDelete(TEntity entity)
        {
            var tmpEntity = CastToBaseEntity(entity);
            var record = CastToBaseEntity(_dbSet.Find(tmpEntity.Id));
            record.RowStatus = ERowStatus.Deleted;
            _dbSet.Update(record as TEntity);
        }

        public TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null)
            => _dbSet.FirstOrDefault(predicate);

        public TEntity? GetLastOrDefault(Expression<Func<TEntity, bool>> predicate = null)
            => _dbSet.Where(predicate).OrderDescending().FirstOrDefault();

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<Pagination<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            uint pageNumber = 0,
            uint pageSize = 10)
        {
            var query = _dbSet.AsQueryable<TEntity>();
            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);
            else
                query = query.OrderBy(p => p);

            var totalRow = (uint)query.Count();

            var list = await query.Skip((int)(pageNumber * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var pagination = new Pagination<TEntity>(pageNumber, pageSize, totalRow, list);

            return pagination;
        }

        public TEntity? GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
            => _dbSet.SingleOrDefault(predicate);

        public void HardDelete(TEntity entity) => _dbSet.Remove(entity);

        public void Insert(TEntity entity) => _dbSet.Add(entity);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public async Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.SingleOrDefaultAsync(predicate);

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<TEntity?> GetLastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null)
            => await _dbSet.Where(predicate).OrderDescending().FirstOrDefaultAsync();

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => _dbSet.AsQueryable<TEntity>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dbSet.AsQueryable().GetEnumerator();

        Type IQueryable.ElementType => _dbSet.AsQueryable<TEntity>().ElementType;

        Expression IQueryable.Expression => _dbSet.AsQueryable<TEntity>().Expression;

        IQueryProvider IQueryable.Provider => _dbSet.AsQueryable<TEntity>().Provider;
    }
} 