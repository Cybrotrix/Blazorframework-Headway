﻿using System;
using System.Collections.Generic;

namespace Headway.Core.Mediators
{
    public class StateNotificationMediator : IStateNotificationMediator
    {
        private readonly Dictionary<string, Action> register;

        public StateNotificationMediator()
        {
            register = new Dictionary<string, Action>();
        }

        public void Register(string target, Action action)
        {
            if(register.ContainsKey(target))
            {
                return;
            }

            register.Add(target, action);
        }

        public void Deregister(string target)
        {
            if (register.ContainsKey(target))
            {
                register.Remove(target);
            }
        }

        public void NotifyStateHasChanged(string target)
        {
            if (register.ContainsKey(target))
            {
                register[target].Invoke();
            }
        }
    }
}
