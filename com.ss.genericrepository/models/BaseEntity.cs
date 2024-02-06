namespace com.ss.genericrepository.models
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public Guid UniqueId { get; set; }
        public ERowStatus RowStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}

