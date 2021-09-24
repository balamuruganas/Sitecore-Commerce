using Sitecore.Foundation.Search.Managers;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using Sitecore.Diagnostics;
using System;
using System.Linq;

namespace Sitecore.Foundation.Cart.Managers
namespace Sitecore.Foundation.Cart.Managers
{
    public class ExtendedCartManager : Sitecore.Commerce.XA.Foundation.Connect.Managers.CartManager, IExtendedCartManager
    {
        public ExtendedCartManager(IConnectServiceProvider connectServiceProvider,
            IStorefrontContext storefrontContext,
            ISearchManager searchManager,
            IExtendedSearchManager extendedSearchManager
            )
            : base(connectServiceProvider, storefrontContext, searchManager)
        {
            this.ExtendedSearchManager = extendedSearchManager;
        }
        public IExtendedSearchManager ExtendedSearchManager { get; protected set; }

        public ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> GetCurrentCart(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string cartName = "Default", bool recalculateTotals = false, StringPropertyCollection propertyBag = null)
        {
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(storefrontContext, "storefrontContext");
            CartResult cartResult = LoadCart(visitorContext, storefrontContext.CurrentStorefront.ShopName, cartName, visitorContext.CustomerId, recalculateTotals, propertyBag);
            return new ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart>(cartResult, cartResult.Cart);
        }
        public ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> MergeCarts(CommerceStorefront storefront, IVisitorContext visitorContext, string anonymousVisitorId, Sitecore.Commerce.Entities.Carts.Cart anonymousVisitorCart, string cartName, StringPropertyCollection propertyBag = null)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(anonymousVisitorId, "anonymousVisitorId");
            Assert.ArgumentNotNull(anonymousVisitorCart, "anonymousVisitorCart");
            string userId = visitorContext.UserId;
            CartResult cartResult = LoadCart(visitorContext, storefront.ShopName, cartName, userId, recalculateTotals: true, propertyBag);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                string systemMessage = StorefrontContext.GetSystemMessage("Cart Not Found Error");
                cartResult.SystemMessages.Add(new SystemMessage
                {
                    Message = systemMessage
                });
                return new ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart>(cartResult, cartResult.Cart);
            }
            CommerceCart commerceCart = (CommerceCart)cartResult.Cart;
            CartResult cartResult2 = new CartResult
            {
                Cart = commerceCart,
                Success = true
            };
            if (!string.Equals(userId, anonymousVisitorId, StringComparison.Ordinal))
            {
                bool flag = (anonymousVisitorCart as CommerceCart)?.OrderForms.Any((CommerceOrderForm of) => of.PromoCodes.Any()) ?? false;
                if (anonymousVisitorCart != null && (anonymousVisitorCart.Lines.Any() || flag) && (string.Equals(commerceCart.ShopName, anonymousVisitorCart.ShopName, StringComparison.Ordinal) || !string.Equals(commerceCart.ExternalId, anonymousVisitorCart.ExternalId, StringComparison.Ordinal)))
                {
                    MergeCartRequest request = new MergeCartRequest(anonymousVisitorCart, commerceCart);
                    request.CopyPropertyBag(propertyBag);
                    cartResult2 = CartServiceProvider.MergeCart(request);
                }
            }
            return new ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart>(cartResult2, cartResult2.Cart);
        }
    }
}