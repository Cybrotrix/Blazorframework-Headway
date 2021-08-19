﻿using Headway.Core.Attributes;
using Headway.Core.Helpers;
using Headway.Core.Interface;
using Headway.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headway.Core.Options
{
    public class ContainerOptionItems : IOptionItems
    {
        public Task<IEnumerable<OptionItem>> GetOptionItemsAsync()
        {
            var containers = TypeAttributeHelper.GetHeadwayTypesByAttribute(typeof(DynamicContainerAttribute));

            var optionItems = from c in containers
                              select new OptionItem
                              {
                                  Id = c.Namespace,
                                  Display = c.DisplayName
                              };

            return Task.FromResult(optionItems);
        }
    }
}
