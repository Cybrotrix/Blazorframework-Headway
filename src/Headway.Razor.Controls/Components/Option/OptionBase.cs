﻿using Headway.Core.Attributes;
using Headway.Core.Constants;
using Headway.Core.Helpers;
using Headway.Core.Model;
using Headway.Core.Notifications;
using Headway.Razor.Controls.Base;
using Headway.RequestApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Headway.Razor.Controls.Components.Option
{
    [DynamicComponent]
    public abstract class OptionBase : DynamicComponentBase
    {
        [Inject]
        public IStateNotification StateNotification { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        protected List<OptionItem> options;

        private bool dynamicOptions;

        public Expression<Func<string>> FieldExpression
        {
            get
            {
                return Expression.Lambda<Func<string>>(Field.MemberExpression);
            }
        }

        public string PropertyValue
        {
            get
            {
                return Field.PropertyInfo.GetValue(Field.Model)?.ToString();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var arg = ComponentArgHelper.GetArg(ComponentArgs, Options.OPTIONS_CODE);

            if(arg == null)
            {
                options = ComponentArgs
                    .Select(a => new OptionItem { Id = a.Name, Display = a.Value.ToString() })
                    .ToList();
            }
            else
            {
                dynamicOptions = true;
            }

            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (dynamicOptions)
            {
                LinkFieldCheck();

                var result = await Mediator.Send(new OptionItemsRequest(ComponentArgs)).ConfigureAwait(false);

                var optionItems = GetResponse(result.OptionItems);

                options = new List<OptionItem>(optionItems);
            }

            await OnInitializedAsync().ConfigureAwait(false);   
        }

        public virtual void OnValueChanged(string value)
        {
            Field.PropertyInfo.SetValue(Field.Model, value);

            if (Field.HasLinkDependents)
            {
                StateNotification.NotifyStateHasChanged(Field.ContainerUniqueId);
            }
        }
    }
}
