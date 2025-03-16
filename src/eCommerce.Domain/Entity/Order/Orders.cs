namespace eCommerce.Domain.Entity.Order;

public class Orders
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
