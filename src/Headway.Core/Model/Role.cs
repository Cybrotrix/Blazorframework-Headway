﻿using Headway.Core.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        [NotMapped]
        public string PermissionList
        {
            get
            {
                if(Permissions == null)
                {
                    return string.Empty;
                }

                return string.Join(", ", 
                    Permissions.Select(p => p.Name)
                    .OrderBy(n => n)
                    .Distinct());
            }
        }

        [NotMapped]
        public string UserList
        {
            get
            {
                if (Users == null)
                {
                    return string.Empty;
                }

                return string.Join(", ", 
                    Users.Select(u => u.Email)
                    .OrderBy(e => e)
                    .Distinct());
            }
        }

        [NotMapped]
        public List<ChecklistItem> PermissionChecklist { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(150, ErrorMessage = "Description must be between 1 and 150 characters")]
        public string Description { get; set; }
    }
}
