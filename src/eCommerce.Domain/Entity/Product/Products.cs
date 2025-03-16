namespace eCommerce.Domain.Entity.Product;

public class Products
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string ImageUrl { get; set; }
}
