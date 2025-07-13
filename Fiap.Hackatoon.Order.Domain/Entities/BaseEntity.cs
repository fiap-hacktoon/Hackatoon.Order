namespace Fiap.Hackatoon.Order.Domain.Entities
{
    public abstract class BaseEntity
    {
        public DateTime Creation { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
