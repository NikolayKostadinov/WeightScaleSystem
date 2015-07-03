using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeightScale.WorkstationsChecker.Web.Areas.PowerUser.Controllers
{
    [Authorize(Roles="Administrator,PowerUser")]
    public abstract class PowerUserController : Controller
    {
        
    }
}