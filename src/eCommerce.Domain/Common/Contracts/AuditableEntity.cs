namespace eCommerce.Domain.Common.Contracts;

public abstract class AuditableEntity : BaseEntity, IAuditableEntity, IEdit, ISoftDelete
{
    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; private set; }

    public Guid? EditLockBy { get; set; }

    public DateTime? EditLockOn { get; private set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; private set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; private set; }

    public int IsDeleted { get; set; }

    protected AuditableEntity()
    {
        CreatedOn = DateTime.UtcNow;
        EditLockOn = LastModifiedOn = DeletedOn = DateTime.UtcNow;
    }
}
