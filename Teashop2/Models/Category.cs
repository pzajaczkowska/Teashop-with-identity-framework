namespace Teashop2.Models
{
    public class Category
    {
        public Category() =>
            Products = new HashSet<Product>();
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
