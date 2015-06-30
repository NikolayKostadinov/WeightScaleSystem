namespace WeightScale.WorkstationsChecker.Data.Identity
{
    using System;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WeightScale.WorkstationsChecker.Model.Identity;

    public class UserStoreIntPk : UserStore<ApplicationUser, ApplicationRole, int,
        UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        public UserStoreIntPk(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
