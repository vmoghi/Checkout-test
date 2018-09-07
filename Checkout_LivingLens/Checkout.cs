using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_LivingLens
{
    public interface ICheckout
    {
        void Scan(string sku);
        double Total();
    }
    public class Checkout : ICheckout
    {
        private readonly IList<ItemModel> _listItems = null;
        private readonly IFakeDatabase _fakeDatabase = null;
        public Checkout(IFakeDatabase fakeDatabase, IList<ItemModel> listItems)
        {
            _fakeDatabase = fakeDatabase;
            _listItems = listItems;
        }
        public void Scan(string sku)
        {
            if (sku == "")
            {
                return;
            }
            foreach (var item in _listItems)
            {
                if (item.Name.ToLower() == sku.ToLower())
                {
                    item.Quantity++;
                    return;
                }
            }
            var itemModel = _fakeDatabase.GetItemModel(sku);
            if (itemModel != null)
            {
                _listItems.Add(itemModel);
            }
        }
        public double Total()
        {
            var totalPrice = 0.00;
            foreach (var item in _listItems)
            {
                if (item.Quantity == 1)
                {
                    totalPrice += item.OriginalPrice;
                    continue;
                }
                try
                {
                    var discountRuleItem = item.DiscountRule;
                    var quantityWithoutDiscount = item.Quantity;
                    if (discountRuleItem != null)
                    {
                        var discountGroups = item.Quantity / discountRuleItem.Quantity;
                        totalPrice += discountGroups * discountRuleItem.DiscountedPrice;
                        quantityWithoutDiscount = item.Quantity % discountRuleItem.Quantity;
                    }
                    totalPrice += quantityWithoutDiscount * item.OriginalPrice;
                }
                catch (Exception)
                {
                }
            }
            return totalPrice;
        }
    }
}
