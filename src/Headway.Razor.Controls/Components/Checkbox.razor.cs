﻿using Headway.Core.Attributes;
using Headway.Razor.Controls.Base;

namespace Headway.Razor.Controls.Components
{
    [DynamicComponent]
    public class CheckboxBase : DynamicComponentBase
    {
        public bool PropertyValue
        {
            get
            {
                return (bool)Field.PropertyInfo.GetValue(Field.Model);
            }
            set
            {
                Field.PropertyInfo.SetValue(Field.Model, value);
            }
        }
    }
}
