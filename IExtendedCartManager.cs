using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using System.Collections.Generic;

namespace Sitecore.Foundation.Cart.Managers
{
    public interface IExtendedCartManager
    {
        ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> GetCurrentCart(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string cartName = "Default", bool recalculateTotals = false, StringPropertyCollection propertyBag = null);
        ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> MergeCarts(CommerceStorefront storefront, IVisitorContext visitorContext, string anonymousVisitorId, Sitecore.Commerce.Entities.Carts.Cart anonymousVisitorCart, string cartName, StringPropertyCollection propertyBag = null);
        ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> AddLineItemsToCart(CommerceStorefront storefront, IVisitorContext visitorContext, Sitecore.Commerce.Entities.Carts.Cart cart, IEnumerable<CartLineArgument> cartLines, StringPropertyCollection propertyBag = null);
    }
}