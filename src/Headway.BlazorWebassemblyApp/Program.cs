using Headway.BlazorWebassemblyApp.Account;
using Headway.Core.Interface;
using Headway.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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

            builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("webapi");
                return new WeatherForecastService(httpClient);
            });

            builder.Services.AddTransient<IMenuService, MenuService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("webapi");
                return new MenuService(httpClient);
            });

            builder.Services.AddOidcAuthentication(options =>
            {
                //// Configure your authentication provider options here.
                //// For more information, see https://aka.ms/blazor-standalone-auth
                //builder.Configuration.Bind("Local", options.ProviderOptions);
                options.ProviderOptions.Authority = "https://localhost:5001/";
                options.ProviderOptions.ClientId = "headwayblazorwebassemblyapp";
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.PostLogoutRedirectUri = "/";
                options.ProviderOptions.ResponseType = "code";
                options.UserOptions.RoleClaim = "role";
            }).AddAccountClaimsPrincipalFactory<UserAccountFactory>();

            await builder.Build().RunAsync();
        }
    }
}
