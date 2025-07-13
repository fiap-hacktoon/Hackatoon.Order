namespace Fiap.Hackatoon.Order.Domain.Entities;

public class ProductDto : BaseEntity
{
    public Guid Id { get; set; }

    public Guid TypeId { get; set; }

    public ProductType Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Removed { get; set; }

    public DateTime? RemovedAt { get; set; }
}