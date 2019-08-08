using System.Collections.Generic;
using BangazonAPI.Models;

namespace BangazonAPI.Controllers
{
    public class Order
    {
        public int Id { get; internal set; }
        public int CustomerId { get; internal set; }
        public int PaymentTypeId { get; internal set; }
        public List<Product> Products { get; internal set; }
        public Customer Customer { get; internal set; }
    }
}