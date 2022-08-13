﻿using Headway.Core.Attributes;
using Headway.Core.Interface;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Headway.Core.Model
{
    [DynamicModel]
    public class MenuItem : IPermissionable
    {
        public int MenuItemId { get; set; }
        public int Order { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "Name must be between 1 and 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Icon is required")]
        [StringLength(30, ErrorMessage = "Icon must be between 1 and 30 characters")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "NavigatePage is required")]
        [StringLength(50, ErrorMessage = "NavigatePage must be between 1 and 50 characters")]
        public string NavigatePage { get; set; }

        [Required(ErrorMessage = "Config is required")]
        [StringLength(20, ErrorMessage = "Config must be between 1 and 20 characters")]
        public string Config { get; set; }

        [Required(ErrorMessage = "Permission is required")]
        [StringLength(20, ErrorMessage = "Permission must be between 1 and 20 characters")]
        public string Permission { get; set; }

        public string NavigateFullPath()
        {
            return $@"{NavigatePage}\{Config}";
        }

        public bool IsPermitted(IEnumerable<string> permissions)
        {
            return permissions.Contains(Permission);
        }
    }
}