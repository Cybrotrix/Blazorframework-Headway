﻿using Headway.Core.Attributes;
using Headway.Core.Constants;
using Headway.Core.Helpers;
using Headway.Razor.Controls.Base;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Headway.Razor.Controls.Components
{
    [DynamicComponent]
    public abstract class IntegerNullableBase : DynamicComponentBase
    {
        protected int? maxLength = null;
        protected int? min = null;
        protected int? max = null;

        protected override Task OnInitializedAsync()
        {
            var maxLengthArg = ComponentArgHelper.GetArg(ComponentArgs, Args.MAX_LENGTH);

            if (maxLengthArg != null)
            {
                maxLength = int.Parse(maxLengthArg.Value);
            }

            var minArg = ComponentArgHelper.GetArg(ComponentArgs, Args.MIN);

            if (minArg != null)
            {
                min = int.Parse(minArg.Value);
            }

            var maxArg = ComponentArgHelper.GetArg(ComponentArgs, Args.MAX);

            if (maxArg != null)
            {
                max = int.Parse(maxArg.Value);
            }

            return base.OnInitializedAsync();
        }

        public int MaxLength { get { return maxLength.HasValue ? maxLength.Value : int.MaxValue; } }
        public int Max { get { return max ?? int.MaxValue; } }
        public int Min { get { return min ?? int.MinValue; } }

        public Expression<Func<int?>> FieldExpression
        {
            get
            {
                return Expression.Lambda<Func<int?>>(Field.MemberExpression);
            }
        }

        public int? PropertyValue
        {
            get
            {
                return (int?)Field.PropertyInfo.GetValue(Field.Model);
            }
            set
            {
                Field.PropertyInfo.SetValue(Field.Model, value);
            }
        }
    }
}
