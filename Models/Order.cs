using System;
using System.Collections.Generic;

namespace PizzeriaApp.Models
{
    public class Order
    {
        public string Id { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }
    }
}