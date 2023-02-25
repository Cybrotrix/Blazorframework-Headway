﻿using Headway.Core.Attributes;
using Headway.Core.Enums;
using Headway.Core.Interface;
using Headway.Core.Model;

namespace Headway.Core.Tests.Helpers
{
    [StateConfiguration]
    public class StateActionHelper : IConfigureState
    {
        public void Configure(State state)
        {
            state.Comment = default;
            state.StateActions.Add(new StateAction { Order = 8, StateActionType = StateActionType.Initialize, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 7, StateActionType = StateActionType.Initialize, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 2, StateActionType = StateActionType.Complete, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 1, StateActionType = StateActionType.Complete, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 4, StateActionType = StateActionType.Start, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 3, StateActionType = StateActionType.Start, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 6, StateActionType = StateActionType.Reset, ActionAsync = StateAction });
            state.StateActions.Add(new StateAction { Order = 5, StateActionType = StateActionType.Reset, ActionAsync = StateAction });
        }

        private Task StateAction(State state, StateActionType stateActionType, int order)
        {
            if (state.Comment == null)
            {
                state.Comment = $"{order} {stateActionType} {state.StateCode}";
            }
            else
            {
                state.Comment += $"; {order} {stateActionType} {state.StateCode}";
            }

            return Task.CompletedTask;
        }
    }
}