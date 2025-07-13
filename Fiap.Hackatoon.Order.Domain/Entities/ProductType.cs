namespace Fiap.Hackatoon.Order.Domain.Entities;

public class ProductType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
