namespace WeigthScale.WebApiHost.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;
    using log4net;

    public class HandleErrorAttribute : ExceptionFilterAttribute
    {
        private readonly ILog logger;

        public HandleErrorAttribute(ILog loggerParam)
        {
            this.logger = loggerParam;
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var actionCtx = actionExecutedContext.ActionContext;
            var controllerCtx = actionCtx.ControllerContext;
            logger.ErrorFormat("Exception occurred. Controller: {0}, action: {1}. Exception message: {2}",
                controllerCtx.ControllerDescriptor.ControllerName,
                actionCtx.ActionDescriptor.ActionName,
                actionExecutedContext.Exception.Message);

            //base.OnException(actionExecutedContext);
        }
    }
}
