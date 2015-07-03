using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using WeightScale.WorkstationsChecker.Web.Infrastructure.IdentityInfrastructure;
using WeightScale.WorkstationsChecker.Model.Identity;

namespace WeightScale.WorkstationsChecker.Web.Infrastructure.Helpers
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, int id)
        {
            var mgr = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return new MvcHtmlString(mgr.FindByIdAsync(id).Result.UserName);
        }
    }
}