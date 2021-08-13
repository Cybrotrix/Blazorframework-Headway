﻿using Headway.Core.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Headway.Core.Model
{
    [DynamicModel]
    public class Role
    {
        public Role()
        {
            Users = new List<User>();
            Permissions = new List<Permission>();
        }

        public int RoleId { get; set; }
        public List<User> Users { get; set; }
        public List<Permission> Permissions { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "Name must be between 1 and 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(30, ErrorMessage = "Description must be between 1 and 30 characters")]
        public string Description { get; set; }
    }
}
