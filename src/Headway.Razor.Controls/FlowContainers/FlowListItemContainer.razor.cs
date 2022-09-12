﻿using Headway.Core.Dynamic;
using Microsoft.AspNetCore.Components;

namespace Headway.Razor.Controls.FlowContainers
{
    public class FlowListItemContainerBase : ComponentBase
    {
        [CascadingParameter]
        public FlowListContainer FlowListContainer { get; set; }

        [Parameter]
        public DynamicContainer Container { get; set; }
    }
}
