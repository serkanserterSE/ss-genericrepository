using com.ss.genericrepository.interfaces;
using com.ss.genericrepository.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace com.ss.genericrepository.concretes
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected DbContext _dbContext;
        private bool _isDisposed = false;
        private Guid _currentUserId = Guid.Empty;

        public UnitOfWork()
        {
        }

        public UnitOfWork SetCurrentUserId(Guid userId) { _currentUserId = userId; return this; }

        public DbContext GetDbContext() => _dbContext;

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
            => new GenericRepository<TEntity>(_dbContext);

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Tracking();
            return _dbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public int SaveChanges()
        {
            Tracking();
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Tracking();
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            Tracking();
            return await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _dbContext.Dispose();
                _dbContext = null;
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        private void Tracking()
        {
            var entries = _dbContext.ChangeTracker.Entries();
            foreach (var entity in entries)
            {
                switch (entity.State)
                {
                    case EntityState.Modified:
                        EntityWhenUpdate(_currentUserId, entity);
                        break;
                    case EntityState.Added:
                        EntityWhenInsert(_currentUserId, entity);
                        break;
                    default:
                        break;
                }
            }
        }

        private void EntityWhenInsert(Guid currentUserId, EntityEntry entity)
        {
            var rowStatus = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "RowStatus");
            if (rowStatus != null)
            {
                rowStatus?.PropertyInfo?.SetValue(entity, Convert.ChangeType(ERowStatus.Active, rowStatus.PropertyInfo.PropertyType), null);
                entity.CurrentValues.SetValues(rowStatus);
            }

            var createdOn = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "CreatedOn");
            if (createdOn != null)
            {
                createdOn?.PropertyInfo?.SetValue(entity, Convert.ChangeType(DateTime.UtcNow, createdOn.PropertyInfo.PropertyType), null);
                entity.CurrentValues.SetValues(createdOn);
            }

            var createdBy = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "CreatedBy");
            if (createdBy != null)
            {
                createdBy?.PropertyInfo?.SetValue(entity, Convert.ChangeType(currentUserId, createdBy.PropertyInfo.PropertyType), null);
                entity.CurrentValues.SetValues(createdBy);
            }
        }

        private void EntityWhenUpdate(Guid currentUserId, EntityEntry entity)
        {
            var rowStatus = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "RowStatus");

            if (rowStatus == null)
                return;

            var currentRowStatus = rowStatus.GetDefaultValue() as ERowStatus?;
            if (currentRowStatus == ERowStatus.Deleted)
                rowStatus?.PropertyInfo?.SetValue(entity, Convert.ChangeType(ERowStatus.Deleted, rowStatus.PropertyInfo.PropertyType), null);
            else
                rowStatus?.PropertyInfo?.SetValue(entity, Convert.ChangeType(ERowStatus.Modified, rowStatus.PropertyInfo.PropertyType), null);
            entity.CurrentValues.SetValues(rowStatus);


            if (currentRowStatus == ERowStatus.Deleted)
            {
                var deletedOn = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "DeletedOn");
                if (deletedOn != null)
                {
                    deletedOn?.PropertyInfo?.SetValue(entity, Convert.ChangeType(DateTime.UtcNow, deletedOn.PropertyInfo.PropertyType), null);
                    entity.CurrentValues.SetValues(deletedOn);
                }

                var deletedBy = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "DeletedBy");
                if (deletedBy != null)
                {
                    deletedBy?.PropertyInfo?.SetValue(entity, Convert.ChangeType(currentUserId, deletedBy.PropertyInfo.PropertyType), null);
                    entity.CurrentValues.SetValues(deletedBy);
                }
            }
            else
            {
                var modifiedOn = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "ModifiedOn");
                if (modifiedOn != null)
                {
                    modifiedOn?.PropertyInfo?.SetValue(entity, Convert.ChangeType(DateTime.UtcNow, modifiedOn.PropertyInfo.PropertyType), null);
                    entity.CurrentValues.SetValues(modifiedOn);
                }

                var modifiedBy = entity.CurrentValues.Properties.FirstOrDefault(p => p.PropertyInfo?.Name == "ModifiedBy");
                if (modifiedBy != null)
                {
                    modifiedBy?.PropertyInfo?.SetValue(entity, Convert.ChangeType(currentUserId, modifiedBy.PropertyInfo.PropertyType), null);
                    entity.CurrentValues.SetValues(modifiedBy);
                }
            }
        }
    }
}