namespace WeightScale.WorkstationsChecker.Web.Models.Identity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class GoogleAuthenticatorViewModel
    {
        [Required]
        public string Code { get; set; }

        public string SecretKey { get; set; }

        public string BarcodeUrl { get; set; }
    }
}