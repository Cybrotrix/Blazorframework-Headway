﻿using Headway.Core.Enums;
using Headway.Core.Extensions;
using Headway.Core.Interface;
using Headway.Core.Model;

namespace Headway.Core.Tests.Helpers
{
    public class FlowHistoryHelper : IConfigureFlow
    {
        public void Configure(Flow flow)
        {
            foreach(var state in flow.States)
            {
                state.Comment = default;
                state.StateActions.Add(new StateAction { StateActionType = StateActionType.Initialize, ActionAsync = StateAction });
                state.StateActions.Add(new StateAction { StateActionType = StateActionType.Completed, ActionAsync = StateAction });
                state.StateActions.Add(new StateAction { StateActionType = StateActionType.Reset, ActionAsync = StateAction });
            }
        }

        private Task StateAction(State state, StateActionType stateActionType, int order)
        {
            switch (stateActionType)
            {
                case StateActionType.Initialize:
                case StateActionType.Completed:
                    state.Owner = Environment.UserName;
                    state.Comment = $"{stateActionType} {state.StateCode}";
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
