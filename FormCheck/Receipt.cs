using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Receipt
{
    public DateTime Date { get; set; }
    public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
    public DiscountCard DiscountCard { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalWithDiscount { get; set; }
}

public class ReceiptItem
{
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
}
