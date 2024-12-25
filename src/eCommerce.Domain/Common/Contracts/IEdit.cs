namespace eCommerce.Domain.Common.Contracts;

public interface IEdit
{
    public Guid? EditLockBy { get; set; }

    public DateTime? EditLockOn { get; }
}
