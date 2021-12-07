﻿using System;
using System.Collections.Generic;

namespace Headway.Core.Dynamic
{
    public class DynamicContainer
    {
        public DynamicContainer()
        {
            Parameters = new Dictionary<string, object>();
            DynamicFields = new List<DynamicField>();
            DynamicContainers = new List<DynamicContainer>();
        }

        public int ContainerId { get; set; }
        public string DynamicContainerTypeName { get; set; }
        public string Label { get; set; }
        public Type DynamicComponent { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public List<DynamicField> DynamicFields {  get; set; }
        public List<DynamicContainer> DynamicContainers {  get; set; }
    }
}
