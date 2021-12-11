﻿using Headway.Core.Attributes;
using Headway.Core.Constants;
using Headway.Core.Dynamic;
using Headway.Razor.Controls.Base;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;

namespace Headway.Razor.Controls.Components
{
    [DynamicContainer]
    public abstract class TabsBase : DynamicContainerBase
    {
        public DynamicContainer activePage { get; set; }

        protected async override Task OnInitializedAsync()
        {
            activePage = Container.DynamicContainers.First();

            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected string GetTabButtonClass(DynamicContainer page)
        {
            return page == activePage ? Css.BTN_PRIMARY : Css.BTN_SECONDARY;
        }

        protected async void SetActivePage(DynamicContainer page)
        {
            await InvokeAsync(() =>
            {
                activePage = page;

                StateHasChanged();
            });
        }
    }
}
