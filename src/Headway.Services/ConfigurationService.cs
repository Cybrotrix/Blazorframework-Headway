﻿using Headway.Core.Constants;
using Headway.Core.Interface;
using Headway.Core.Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace Headway.Services
{
    public class ConfigurationService : ServiceBase, IConfigurationService
    {
        private readonly IConfigCache configCache;

        public ConfigurationService(HttpClient httpClient, IConfigCache configCache)
            : base(httpClient, false)
        {
            this.configCache = configCache;
        }

        public ConfigurationService(HttpClient httpClient, TokenProvider tokenProvider, IConfigCache configCache)
            : base(httpClient, true, tokenProvider)
        {
            this.configCache = configCache;
        }

        public async Task<IResponse<Config>> GetConfigAsync(string name)
        {
            var config = configCache.GetConfig(name);

            if(config != null)
            {
                return GetResponseResult<Config>(config);
            }

            var httpResponseMessage = await httpClient.GetAsync($"{Controllers.CONFIGURATION}/{name}").ConfigureAwait(false);

            var response = await GetResponseAsync<Config>(httpResponseMessage).ConfigureAwait(false);

            if(response.IsSuccess)
            {
                configCache.AddConfig(response.Result);
            }

            return response;
        }
    }
}
