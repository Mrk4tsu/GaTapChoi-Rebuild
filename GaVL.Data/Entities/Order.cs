using GaVL.Data.EntityTypes;

namespace GaVL.Data.Entities
{
    public class Order
    {
        public string Id { get; set; }
        public decimal Total { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NumberPhone { get; set; }
        public PaymentType PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
