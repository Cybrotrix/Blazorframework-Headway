using Headway.BlazorWebassemblyApp.Account;
using Headway.Core.Cache;
using Headway.Core.Interface;
using Headway.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Headway.BlazorWebassemblyApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("OidcConfiguration", options.ProviderOptions);
                options.UserOptions.RoleClaim = "role";
            }).AddAccountClaimsPrincipalFactory<UserAccountFactory>();

            builder.Services.AddHttpClient("webapi", (sp, client) =>
            {
                client.BaseAddress = new Uri("https://localhost:44320");
            }).AddHttpMessageHandler(sp =>
            {
                var handler = sp.GetService<AuthorizationMessageHandler>()
                .ConfigureHandler(
                    authorizedUrls: new[] { "https://localhost:44320" },
                    scopes: new[] { "webapi" });
                return handler;
            });

            builder.Services.AddSingleton<IConfigCache, ConfigCache>();
            builder.Services.AddSingleton<IDynamicTypeCache, DynamicTypeCache>();
            builder.Services.AddSingleton<IDynamicConfigService, DynamicConfigService>();

            builder.Services.AddTransient<IModuleService, ModuleService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("webapi");
                return new ModuleService(httpClient);
            });

            builder.Services.AddTransient<IAuthorisationService, AuthorisationService>(sp =>
            {
                var dynamicConfigService = sp.GetRequiredService<IDynamicConfigService>();
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("webapi");
                return new AuthorisationService(httpClient, dynamicConfigService);
            });

            builder.Services.AddTransient<IConfigurationService, ConfigurationService>(sp =>
            {
                var configCache = sp.GetRequiredService<IConfigCache>();
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("webapi");
                return new ConfigurationService(httpClient, configCache);
            });

            await builder.Build().RunAsync();
        }
    }
}
