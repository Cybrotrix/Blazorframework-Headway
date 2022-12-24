﻿using Headway.Core.Enums;
using Headway.Core.Interface;
using Headway.Core.Model;

namespace Headway.Core.Tests.Helpers
{
    public class StateRoutingHelper : IConfigureState
    {
        public void Configure(State state)
        {
            state.Context = null;
            state.StateActions.Add(new StateAction { Order = 1, StateActionType = StateActionType.Complete, ActionAsync = StateAction });
        }

        private Task StateAction(State state, StateActionType stateActionType, int order)
        {
            var lastStatePosition = state.Flow.States.Max(s => s.Position);
            var lastState = state.Flow.States.Single(s => s.Position.Equals(lastStatePosition));
            state.Transitions.Clear();
            state.Transitions.Add(lastState);
            state.Context = $"Route {state.StateCode} to {lastState.StateCode}";
            lastState.Context = $"Routed from {state.StateCode}";
            return Task.CompletedTask;
        }
    }
}