namespace GaVL.DTO.Payments
{
    public class CreateOrderDto
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
        public string Email { get; set; }
        public string? NumberPhone { get; set; }
    }
}
