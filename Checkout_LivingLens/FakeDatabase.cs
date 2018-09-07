using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_LivingLens
{
    public interface IFakeDatabase
    {
        ItemModel GetItemModel(string itemName);
    }
    public class FakeDatabase : IFakeDatabase
    {
        private Dictionary<string, Dictionary<double, string>> _context = null;
        public FakeDatabase()
        {
            _context = new Dictionary<string, Dictionary<double, string>>();
            var item_Apple = new Dictionary<double, string>();
            item_Apple.Add(0.50, "");
            _context.Add("apple", item_Apple);
          
            var item_Bananas = new Dictionary<double, string>();
            item_Bananas.Add(0.70, "2 for 1");
            _context.Add("bananas", item_Bananas);

            var item_Orange = new Dictionary<double, string>();
            item_Orange.Add(0.45, "3 for 0.90");
            _context.Add("orange", item_Orange);
        }

        public ItemModel GetItemModel(string itemName)
        {
            ItemModel itemModel = null;
            if (itemName == "")
            {
                return itemModel;
            }
            try
            {
                DiscountRuleModel discountRule = null;
                var itemNameLower = itemName.ToLower();
                var itemData = _context[itemNameLower].First();
                var originalItemPrice = itemData.Key;
                var priceRule = itemData.Value;
                if (!string.IsNullOrEmpty(priceRule))
                {
                    discountRule = new DiscountRuleModel();
                    var splitTextOffer = priceRule.Split(' ');
                    var minQuantityForDiscount = int.Parse(splitTextOffer[0]);
                    var discountedPrice = double.Parse(splitTextOffer[2]);
                    discountRule.Quantity = minQuantityForDiscount;
                    discountRule.DiscountedPrice = discountedPrice;
                }
                itemModel = new ItemModel();
                itemModel.Name = itemName;
                itemModel.OriginalPrice = originalItemPrice;
                itemModel.Quantity = 1;
                itemModel.DiscountRule = discountRule;
            }
            catch (Exception)
            {
            }
            return itemModel;
        }
    }
}
