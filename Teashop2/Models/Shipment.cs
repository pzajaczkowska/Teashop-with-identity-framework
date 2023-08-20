namespace Teashop2.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string? EstimatedTime { get; set; }

        //public ICollection<Order> Orders { get; set; }
    }
}
