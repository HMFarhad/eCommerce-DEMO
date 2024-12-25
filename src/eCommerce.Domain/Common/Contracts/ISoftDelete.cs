namespace eCommerce.Domain.Common.Contracts;

public interface ISoftDelete
{
    DateTime? DeletedOn { get; }

    Guid? DeletedBy { get; set; }

    int IsDeleted { get; set; }
}
