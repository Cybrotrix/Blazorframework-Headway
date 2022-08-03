﻿using Headway.Core.Attributes;
using Headway.Core.Helpers;
using Headway.Core.Interface;
using Headway.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headway.Core.Options
{
    public class SearchItemComponentOptionItems : IOptionItems
    {
        public Task<IEnumerable<OptionItem>> GetOptionItemsAsync(IEnumerable<Arg> args)
        {
            var components = TypeAttributeHelper.GetHeadwayTypesByAttribute(typeof(DynamicSearchItemComponentAttribute));

            List<OptionItem> optionItems = new() { new OptionItem() };

            optionItems.AddRange((from c in components
                              orderby c.Name
                              select new OptionItem
                              {
                                  Id = c.Namespace,
                                  Display = c.DisplayName
                              }).ToList());

            return Task.FromResult(optionItems.AsEnumerable());
        }
    }
}
