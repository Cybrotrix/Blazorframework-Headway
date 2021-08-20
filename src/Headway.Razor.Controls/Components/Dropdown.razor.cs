﻿using Headway.Core.Attributes;
using Headway.Core.Interface;
using Headway.Core.Model;
using Headway.Razor.Controls.Base;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Headway.Razor.Controls.Components
{
    [DynamicComponent]
    public class DropdownBase : DynamicComponentBase
    {
        [Inject]
        public IOptionsService OptionsService { get; set; }

        protected IEnumerable<OptionItem> OptionItems;

        protected override async Task OnParametersSetAsync()
        {
            var args = ComponentArgs.Select(a => a.Value.ToString()).ToArray();

            var result = await OptionsService.GetOptionItemsAsync(args).ConfigureAwait(false);

            OptionItems = GetResponse(result);

            await base.OnParametersSetAsync();
        }
    }
}
