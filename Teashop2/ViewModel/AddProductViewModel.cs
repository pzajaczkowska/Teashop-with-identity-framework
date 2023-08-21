using Teashop2.Models;

namespace Teashop2.ViewModel
{
    public class AddProductViewModel
    {
        public Product Product { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
    }

}
