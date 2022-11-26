﻿using Headway.Core.Helpers;
using static MudBlazor.Icons;

namespace Headway.Blazor.Controls.Helpers
{
    public static class IconHelper
    {
        private readonly static Outlined outlined = new();
        private readonly static DynamicTypeHelper<Outlined> outlinedHelper;

        static IconHelper()
        {
            outlinedHelper = DynamicTypeHelper.Get<Outlined>();
        }

        public static string GetOutlined(string name)
        {
            if(outlinedHelper == null)
            {
                return string.Empty;
            }

            return outlinedHelper.GetValue(outlined, name).ToString();
        }
    }
}
