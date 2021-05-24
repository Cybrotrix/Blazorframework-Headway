﻿using Headway.Core.Interface;
using Headway.Core.Model;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Headway.Services
{
    public class AuthorisationService : ServiceBase, IAuthorisationService
    {
        public AuthorisationService(HttpClient httpClient, NavigationManager navigationManager)
            : base(httpClient, navigationManager, false)
        {
        }

        public AuthorisationService(HttpClient httpClient, NavigationManager navigationManager, TokenProvider tokenProvider)
            : base(httpClient, navigationManager, true, tokenProvider)
        {
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var httpResponseMessage = await httpClient.GetAsync($"Users").ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<User>>
                    (await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var httpResponseMessage = await httpClient.GetAsync($"Users/{userId}").ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await JsonSerializer.DeserializeAsync<User>
                    (await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"Users", user).ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await httpResponseMessage.Content.ReadFromJsonAsync<User>(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var httpResponseMessage = await httpClient.PutAsJsonAsync($"Users", user).ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await httpResponseMessage.Content.ReadFromJsonAsync<User>(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var httpResponseMessage = await httpClient.DeleteAsync($"Users/{userId}").ConfigureAwait(false);

            IsSuccessStatusCode(httpResponseMessage);
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            var httpResponseMessage = await httpClient.GetAsync($"Permissions").ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<Permission>>
                    (await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<Permission> GetPermissionAsync(int permissionId)
        {
            var httpResponseMessage = await httpClient.GetAsync($"Permissions/{permissionId}").ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await JsonSerializer.DeserializeAsync<Permission>
                    (await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<Permission> AddPermissionAsync(Permission permission)
        {
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"Permissions", permission).ConfigureAwait(false);

            if(IsSuccessStatusCode(httpResponseMessage))
            {
                return await httpResponseMessage.Content.ReadFromJsonAsync<Permission>(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);

            }

            return null;
        }

        public async Task<Permission> UpdatePermissionAsync(Permission permission)
        {
            var httpResponseMessage = await httpClient.PutAsJsonAsync($"Permissions", permission).ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await httpResponseMessage.Content.ReadFromJsonAsync<Permission>(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);

            }

            return null;
        }

        public async Task DeletePermissionAsync(int permissionId)
        {
            var httpResponseMessage = await httpClient.DeleteAsync($"Permissions/{permissionId}").ConfigureAwait(false);

            IsSuccessStatusCode(httpResponseMessage);
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var httpResponseMessage = await httpClient.GetAsync($"Roles").ConfigureAwait(false);

            if(IsSuccessStatusCode(httpResponseMessage))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<Role>>
                    (await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<Role> GetRoleAsync(int roleId)
        {
            var httpResponseMessage = await httpClient.GetAsync($"Roles/{roleId}").ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await JsonSerializer.DeserializeAsync<Role>
                    (await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<Role> AddRoleAsync(Role role)
        {
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"Roles", role).ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await httpResponseMessage.Content.ReadFromJsonAsync<Role>
                    (new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            var httpResponseMessage = await httpClient.PutAsJsonAsync($"Roles", role).ConfigureAwait(false);

            if (IsSuccessStatusCode(httpResponseMessage))
            {
                return await httpResponseMessage.Content.ReadFromJsonAsync<Role>
                    (new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);
            }

            return null;
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            var httpResponseMessage = await httpClient.DeleteAsync($"Roles/{roleId}").ConfigureAwait(false);

            IsSuccessStatusCode(httpResponseMessage);
        }
    }
}