namespace WeightScale.WorkstationsChecker.Data
{
    using System;
    using System.Linq;
    using FuelOrderingSystem.Data;
    using WeightScale.WorkstationsChecker.Model.Identity;

    public class PrincipalInfo : IPrincipalInfo
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public ApplicationUser User { get; set; }

    }
}
