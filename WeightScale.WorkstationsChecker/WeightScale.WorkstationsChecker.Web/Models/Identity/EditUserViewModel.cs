namespace WeightScale.WorkstationsChecker.Web.Models.Identity
{
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using WeightScale.WorkstationsChecker.Model.Identity;
    using WeightScale.WorkstationsChecker.Web.Infrastructure.Mappings;

    public class EditUserViewModel:IMapFrom<ApplicationUser>
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [Display(Name = "Електронна поща")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Нова парола")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

    }
}