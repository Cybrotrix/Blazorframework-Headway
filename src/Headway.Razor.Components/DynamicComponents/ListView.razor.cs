﻿using Headway.Core.Attributes;
using Headway.Core.Dynamic;
using Headway.Core.Interface;
using Headway.Core.Model;
using Headway.Razor.Components.Base;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Headway.Razor.Components.DynamicComponents
{
    [DynamicComponent]
    public abstract class ListViewBase<T> : HeadwayComponentBase
    {
        [Inject]
        public IDynamicService DynamicService { get; set; }

        [Parameter]
        public string Config { get; set; }

        protected DynamicList<T> dynamicList;

        protected IEnumerable<ConfigItem> configItems;

        protected override async Task OnInitializedAsync()
        {
            var result = 
                await DynamicService.GetDynamicListAsync<T>(Config)
                .ConfigureAwait(false);

            dynamicList = GetResponse(result);

            configItems = dynamicList.Config.ConfigItems;

            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected void Add()
        {
            NavigationManager.NavigateTo($"{dynamicList.Config.NavigateTo}/{dynamicList.Config.NavigateToConfig}");
        }

        protected void Update(object id)
        {
            NavigationManager.NavigateTo($"{dynamicList.Config.NavigateTo}/{dynamicList.Config.NavigateToConfig}/{id}");
        }
    }
}
