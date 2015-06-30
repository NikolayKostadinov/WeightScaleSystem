namespace WeightScale.WorkstationsChecker.Model.Identity
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class RoleIntPk : IdentityRole<int, UserRoleIntPk>
    {
        public RoleIntPk()
        {
        }

        public RoleIntPk(string name)
        {
            Name = name;
        }
    }
}