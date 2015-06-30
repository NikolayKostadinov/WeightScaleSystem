using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.WorkstationsChecker.Model.Identity;

namespace FuelOrderingSystem.Data
{
    public interface IPrincipalInfo
    {
        ApplicationUser User { get; set; }
    }
}
