using System.Web.Mvc;

namespace WeightScale.WorkstationsChecker.Web.Areas.PowerUser
{
    public class PowerUserAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PowerUser";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PowerUser_default",
                "PowerUser/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}