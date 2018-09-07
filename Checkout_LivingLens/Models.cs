using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_LivingLens
{
    public class ItemModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double OriginalPrice { get; set; }
        public DiscountRuleModel DiscountRule { get; set; }
    }

    public class DiscountRuleModel
    {
        public int Quantity { get; set; }
        public double DiscountedPrice { get; set; }
    }
}
