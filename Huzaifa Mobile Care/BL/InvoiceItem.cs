using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huzaifa_Mobile_Care.BL
{
    class InvoiceItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Cost { get; set; }
        public int Margin { get; set; }
        public InvoiceItem()
        {
            Name = null;
            Quantity = 1;
            Price = 0;
            Cost = 0;
            Margin = 0;
        }
        public InvoiceItem(string name, int quantity, int price, int cost, int margin)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            Cost = cost;
            Margin = margin;
        }
    }
}
