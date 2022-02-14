﻿using Headway.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Headway.Core.Interface
{
    public interface IConfigurationService
    { 
        Task<IServiceResult<Config>> GetConfigAsync(string name);
    }
}
