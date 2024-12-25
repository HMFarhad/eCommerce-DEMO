using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Domain.Common.Contracts;

public abstract class BaseEntity : BaseEntity<long>
{
    protected BaseEntity()
    {
    }
}

public abstract class BaseEntity<TId> : IEntity<TId>
{
    public TId Id { get; set; } = default!;

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new();
}