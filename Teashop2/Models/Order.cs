using Teashop2.Areas.Identity.Data;

namespace Teashop2.Models
{
    public enum Status
    {
        CART,
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

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public ICollection<OrderedProduct> OrderedProducts { get; set; }
    }
}
