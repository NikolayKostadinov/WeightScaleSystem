﻿namespace WeightScale.WorkstationsChecker.Web.Models.Identity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using WeightScale.WorkstationsChecker.Model.Identity;
    using WeightScale.WorkstationsChecker.Web.Infrastructure.Mappings;

    public class CreateUserViewModel : IMapFrom<ApplicationUser>
    {
        [Required]
        [Display(Name = "Потребителско име")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Електронна поща")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Парола")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Повторно въвеждане на паролата")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Въведените пароли трябва да съвпадат !!!")]
        public string ConfirmPassword { get; set; }
    }
}