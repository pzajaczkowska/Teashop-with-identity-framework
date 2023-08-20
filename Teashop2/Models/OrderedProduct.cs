namespace Teashop2.Models
{
    public class OrderedProduct
    {
        public int Id { get; set; }
        public float? PricePerUnit { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
