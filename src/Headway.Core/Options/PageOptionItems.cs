﻿using Headway.Core.Attributes;
using Headway.Core.Helpers;
using Headway.Core.Interface;
using Headway.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headway.Core.Options
{
    public class PageOptionItems : IOptionItems
    {
        public Task<IEnumerable<OptionItem>> GetOptionItemsAsync()
        {
            var components = TypeAttributeHelper.GetHeadwayTypesByAttribute(typeof(DynamicPageAttribute));

            var optionItems = from c in components
                              select new OptionItem
                              {
                                  Id = c.Name,
                                  Display = c.DisplayName
                              };

            return Task.FromResult(optionItems);
        }
    }
}
