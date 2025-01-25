using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ReceiptBuilder
{
    private Receipt _receipt = new Receipt();

    public ReceiptBuilder AddProduct(Product product, int quantity)
    {
        var item = new ReceiptItem
        {
            Product = product,
            Quantity = quantity,
            Total = product.Price * quantity
        };

        if (product.IsWholesale && quantity >= 5)
        {
            item.Discount = item.Total * 0.10m;
        }
        else if (_receipt.DiscountCard != null)
        {
            item.Discount = item.Total * (_receipt.DiscountCard.DiscountPercentage / 100);
        }

        _receipt.Items.Add(item);
        return this;
    }

    public ReceiptBuilder SetDiscountCard(DiscountCard card)
    {
        _receipt.DiscountCard = card;
        return this;
    }

    public Receipt Build()
    {
        _receipt.Date = DateTime.Now;
        _receipt.TotalPrice = _receipt.Items.Sum(item => item.Total);
        _receipt.TotalDiscount = _receipt.Items.Sum(item => item.Discount);
        _receipt.TotalWithDiscount = _receipt.TotalPrice - _receipt.TotalDiscount;
        return _receipt;
    }
}