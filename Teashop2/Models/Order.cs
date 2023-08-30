using Microsoft.AspNetCore.Identity;
using Teashop2.Areas.Identity.Data;

namespace Teashop2.Models
{
    public enum Status
    {
        PAID,
        COMPLETING,
        SHIPPED
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public float TotalCost { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNo { get; set; }
        public string Address { get => $"ul. {Street} {HouseNo}, {PostalCode} {City}";} 

        public IdentityUser User{ get; set; }
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public ICollection<OrderedProduct> OrderedProducts { get; set; } = new List<OrderedProduct>();
    }
}
