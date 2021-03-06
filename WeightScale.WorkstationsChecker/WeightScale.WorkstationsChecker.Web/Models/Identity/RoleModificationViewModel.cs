﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeightScale.WorkstationsChecker.Web.Models.Identity
{
    public class RoleModificationViewModel
    {
        [Required]
        public string RoleName { get; set; }

        [Required]
        public bool IsAvailableForAdministrators { get; set; }

        public int[] IdsToAdd { get; set; }

        public int[]IdsToDelete { get; set; }
    }
}