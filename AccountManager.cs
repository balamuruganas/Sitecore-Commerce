using Sitecore.Commerce;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using Sitecore.Diagnostics;
using Sitecore.Security.Authentication;
namespace Sitecore.Foundation.Cart.Managers
{
    public class AccountManager : Sitecore.Commerce.XA.Foundation.Connect.Managers.AccountManager
    {
        public IExtendedCartManager ExtendedCartManager { get; set; }
        public AccountManager(IConnectServiceProvider connectServiceProvider,
            Sitecore.Commerce.XA.Foundation.Connect.Managers.ICartManager cartManager,
            IStorefrontContext storefrontContext, IModelProvider modelProvider,
            IExtendedCartManager extendedCartManager) :
            base(connectServiceProvider, cartManager, storefrontContext, modelProvider)
        {
            this.ExtendedCartManager = extendedCartManager;
        }

        public override bool Login(IStorefrontContext storefront, IVisitorContext visitorContext, string userName, string password, bool persistent, StringPropertyCollection propertyBag = null)
        {

            Assert.ArgumentNotNullOrEmpty(userName, "userName");
            Assert.ArgumentNotNullOrEmpty(password, "password");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(storefront, "storefront");
            string customerId = visitorContext.CustomerId;
            Sitecore.Commerce.Entities.Carts.Cart result = CartManager.GetCurrentCart(visitorContext, storefront).Result;
            Sitecore.Commerce.Entities.Carts.Cart result2 = ExtendedCartManager.GetCurrentCart(visitorContext, storefront, "Favorite_").Result;
            bool flag = AuthenticationManager.Login(userName, password, persistent);
            if (flag)
            {
                try
                {
                    CommerceTracker.Current.IdentifyAs("CommerceUser", userName);
                }
                catch (Sitecore.Analytics.DataAccess.XdbUnavailableException)
                {
                    Log.Warn("The Storefront has detected that xDB is no longer available during the login.  This has not stopped the processing and the user was allowed to continue.", this);
                }
                visitorContext.UserJustLoggedIn();
                CartManager.MergeCarts(storefront.CurrentStorefront, visitorContext, customerId, result, propertyBag);
                ExtendedCartManager.MergeCarts(storefront.CurrentStorefront, visitorContext, customerId, result2, "Favorite_", propertyBag);
            }
            return flag;
        }
    }
}