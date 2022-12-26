﻿using Headway.Core.Enums;
using Headway.Core.Exceptions;
using Headway.Core.Interface;
using Headway.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headway.Core.Extensions
{
    public static class StateExtensions
    {
        private static readonly IDictionary<string, StateConfiguration> stateConfigurationCache = new Dictionary<string, StateConfiguration>();
        private static object stateConfigurationCacheLock = new object();

        public static async Task InitialiseAsync(this State state)
        {
            if(state.StateStatus.Equals(StateStatus.InProgress)) 
            {
                throw new StateException(state, $"Can't initialize {state.StateStatus} because it's already {StateStatus.InProgress}.");
            }

            await state.ExecuteActionsAsync(StateActionType.Initialize).ConfigureAwait(false);

            if (state.SubStates.Any()) 
            {
                var subState = state.SubStates.FirstState();

                await subState.InitialiseAsync().ConfigureAwait(false);
            }

            state.StateStatus = StateStatus.InProgress;

            state.Flow.History.RecordHistory(state);

            if(!state.SubStates.Any())
            {
                if(state.Flow.ActiveState != state)
                {
                    state.Flow.ActiveState = state;
                }
            }

            if(state.StateType.Equals(StateType.Auto))
            {
                await state.CompleteAsync().ConfigureAwait(false);
            }
        }

        public static async Task CompleteAsync(this State state, string transitionStateCode = "")
        {
            if (state.StateStatus.Equals(StateStatus.Completed))
            {
                throw new StateException(state, $"Can't complete {state.StateCode} because it's already {StateStatus.Completed}.");
            }

            if (state.StateStatus.Equals(StateStatus.NotStarted))
            {
                throw new StateException(state, $"Can't complete {state.StateCode} because it's still {StateStatus.NotStarted}.");
            }

            var uncompletedSubStates = state.SubStates.Where(s => s.StateStatus != StateStatus.Completed).ToList();

            if(uncompletedSubStates.Any())
            {
                var uncompletedSubStateDescriptions = uncompletedSubStates.Select(s => $"{s.StateCode}={s.StateStatus}");
                var joinedDescriptions = string.Join(",", uncompletedSubStateDescriptions);
                throw new StateException(state, $"Can't complete {state.StateCode} because sub states not yet {StateStatus.Completed} : {joinedDescriptions}.");
            }

            if (!string.IsNullOrWhiteSpace(transitionStateCode)
                && !state.Transitions.Any(s => s.StateCode.Equals(transitionStateCode)))
            {
                throw new StateException(state, $"Can't complete {state.StateCode} because it doesn't support transitioning to {transitionStateCode}.");
            }

            await state.ExecuteActionsAsync(StateActionType.Complete).ConfigureAwait(false);

            State transitionState = null;

            if (string.IsNullOrWhiteSpace(transitionStateCode))
            {
                transitionState = state.Transitions.FirstOrDefault();
            }
            else
            {
                transitionState = state.Transitions.FirstOrDefault(s => s.StateCode.Equals(transitionStateCode));

                if (transitionState == null)
                {
                    throw new StateException(state, $"Can't complete {state.StateCode} because it doesn't support transitioning to {transitionStateCode}.");
                }
            }

            state.StateStatus = StateStatus.Completed;

            state.Flow.History.RecordHistory(state);

            if (transitionState != null)
            {
                await transitionState.InitialiseAsync().ConfigureAwait(false);
            }
            else
            {
                await state.ParentState.CompleteAsync().ConfigureAwait(false);
            }
        }

        public static async Task ResestAsync(this State state, string transitionStateCode = "")
        {
            if (!string.IsNullOrWhiteSpace(transitionStateCode)
                && !state.Transitions.Any(s => s.StateCode.Equals(transitionStateCode)))
            {
                throw new StateException(state, $"Can't reset {state.StateCode} because it doesn't support transitioning to {transitionStateCode}.");
            }

            await state.ExecuteActionsAsync(StateActionType.Reset).ConfigureAwait(false);

            state.StateStatus = default;
            state.Owner = default;

            if (!string.IsNullOrWhiteSpace(transitionStateCode))
            {
                if (!state.Transitions.Any(s => s.StateCode.Equals(transitionStateCode)))
                {
                    throw new StateException(state, $"Can't reset {state.StateCode} because it doesn't support transitioning to {transitionStateCode}.");
                }

                var resetState = state.Transitions.First(s => s.StateCode.Equals(transitionStateCode));

                if(resetState.Position > state.Position)
                {
                    throw new StateException(state, $"Can't reset to {resetState.StateCode} (position {resetState.Position}) because it is positioned after {state.StateCode} (position {state.Position}).");
                }

                var resetStates = state.Flow.States.Where(
                    s => s.Position >= resetState.Position && s.Position < state.Position).ToList();

                foreach (var rs in resetStates.OrderByDescending(s => s.Position))
                {
                    await rs.ResestAsync().ConfigureAwait(false);
                }

                await resetState.InitialiseAsync().ConfigureAwait(false);
            }
        }

        public static async Task ExecuteActionsAsync(this State state, StateActionType stateFunctionType)
        {
            if (state.StateActions == null)
            {
                return;
            }

            if (!state.Configured)
            {
                state.Configure();
            }

            var actions = state.StateActions
                .Where(a => a.StateActionType.Equals(stateFunctionType))
                .OrderBy(a => a.Order)
                .ToList();

            foreach (var action in actions)
            {
                await action.ActionAsync(state, action.StateActionType, action.Order).ConfigureAwait(false);
            }
        }

        public static State FirstState(this List<State> states)
        {
            var firstPosition = states.Min(s => s.Position);

            return states.First(s => s.Position.Equals(firstPosition));
        }

        public static void Configure(this State state)
        {
            if(state.Configured)
            {
                throw new StateException(state, $"{state.StateCode} has already been configured.");
            }

            if (!string.IsNullOrWhiteSpace(state.ConfigureStateClass))
            {
                StateConfiguration stateConfiguration = null;

                lock (stateConfigurationCacheLock)
                {
                    if (stateConfigurationCache.TryGetValue(
                        state.ConfigureStateClass, out StateConfiguration existingStateConfiguration))
                    {
                        stateConfiguration = existingStateConfiguration;
                    }
                    else
                    {
                        var type = Type.GetType(state.ConfigureStateClass);

                        if (type == null)
                        {
                            throw new StateException(state, $"Can't resolve {state.ConfigureStateClass}");
                        }

                        var instance = (IConfigureState)Activator.CreateInstance(type);

                        var methodInfo = type.GetMethod("Configure");

                        stateConfiguration = new StateConfiguration
                        {
                            Instance = instance,
                            MethodInfo = methodInfo
                        };

                        stateConfigurationCache.Add(state.ConfigureStateClass, stateConfiguration);
                    }
                }

                stateConfiguration.Configure(state);

                state.Configured = true;
            }
        }
    }
}