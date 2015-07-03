namespace WeightScale.WorkstationsChecker.Data.Identity
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WeightScale.WorkstationsChecker.Model.Identity;

    public class RoleStoreIntPk : RoleStore<ApplicationRole, int, UserRoleIntPk>, IRoleStore<ApplicationRole,int>
    {
        public RoleStoreIntPk(ApplicationDbContext context) : base(context)
        {
        }
    }
}