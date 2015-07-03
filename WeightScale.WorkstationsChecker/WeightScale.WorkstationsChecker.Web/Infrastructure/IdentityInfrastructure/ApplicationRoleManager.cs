namespace WeightScale.WorkstationsChecker.Web.Infrastructure.IdentityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using WeightScale.WorkstationsChecker.Data.Identity;
    using WeightScale.WorkstationsChecker.Model.Identity;

    public class ApplicationRoleManager : RoleManager<ApplicationRole,int>, IDisposable
    {
        public ApplicationRoleManager(RoleStoreIntPk store)
            :base(store)
        {
            
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStoreIntPk(context.Get<ApplicationDbContext>()));
        }
    }
}