namespace Teashop2.Models
{
    public class ShoppingCart
    {
        public List<OrderedProduct> Products { get; set; } = new List<OrderedProduct>();
    }
}
