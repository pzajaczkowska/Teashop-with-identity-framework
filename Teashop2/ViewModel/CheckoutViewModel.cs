using Teashop2.Models;

namespace Teashop2.ViewModels
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<Shipment> Shipments { get; set; }
    }
}
