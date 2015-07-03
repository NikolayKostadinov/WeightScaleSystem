using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeightScale.WorkstationsChecker.Model.Identity;

namespace WeightScale.WorkstationsChecker.Web.Models.Identity
{
    public class RoleEditViewModel
    {
        public ApplicationRole Role { get; set; }

        public bool IsAvailableForAdministrators { get { return Role.IsAvailableForAdministrators; } }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }
}