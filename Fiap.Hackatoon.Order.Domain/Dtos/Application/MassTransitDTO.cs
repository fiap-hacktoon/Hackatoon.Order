namespace Fiap.Hackatoon.Order.Domain.Dtos.Application
{
    public class MassTransitDTO
    { 
        public Queues QueueList { get; set; }

        public string Server { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public ushort Port { get; set; }
    }

    public class Queues
    {
        public string InsertQueue { get; set; }

        public string UpdateQueue { get; set; }

        public string DeleteQueue { get; set; }
    }
}
