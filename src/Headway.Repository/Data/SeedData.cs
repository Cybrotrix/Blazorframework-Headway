﻿using Headway.Core.Model;
using System.Linq;

namespace Headway.Repository.Data
{
    public class SeedData
    {
        public static void Initialise(ApplicationDbContext applicationDbContext)
        {
            Permission user = null;
            Permission admin = null;

            if (!applicationDbContext.Users.Any()
                && !applicationDbContext.Permissions.Any()
                && !applicationDbContext.Roles.Any())
            {
                admin = new Permission { Name = "Admin", Description = "Administrator" };
                user = new Permission { Name = "User", Description = "Headway User" };
                applicationDbContext.Permissions.Add(admin);
                applicationDbContext.Permissions.Add(user);
                applicationDbContext.SaveChanges();

                var alice = new User { UserName = "alice", Email = "alice@email.com" };
                var bob = new User { UserName = "bob", Email = "bob@email.com" };
                var jane = new User { UserName = "jane", Email = "jane@email.com" };
                var will = new User { UserName = "will", Email = "will@email.com" };
                applicationDbContext.Users.Add(alice);
                applicationDbContext.Users.Add(bob);
                applicationDbContext.Users.Add(jane);
                applicationDbContext.Users.Add(will);
                applicationDbContext.SaveChanges();

                var adminRole = new Role { Name = "Admin", Description = "Administrator Role" };
                var userRole = new Role { Name = "User", Description = "Headway User Role" };
                applicationDbContext.Roles.Add(adminRole);
                applicationDbContext.Roles.Add(userRole);
                applicationDbContext.SaveChanges();

                adminRole.Permissions.Add(admin);
                userRole.Permissions.Add(user);
                applicationDbContext.SaveChanges();

                alice.Roles.Add(adminRole);
                alice.Roles.Add(userRole);
                bob.Roles.Add(adminRole);
                bob.Roles.Add(userRole);
                jane.Roles.Add(userRole);
                will.Roles.Add(userRole);
                applicationDbContext.SaveChanges();
            }

            if (!applicationDbContext.Modules.Any()
                && !applicationDbContext.Categories.Any()
                && !applicationDbContext.MenuItems.Any())
            {
                var home = new Module { Name = "Home", Order = 1, Permission = user.Name };
                var administration = new Module { Name = "Administration", Order = 2, Permission = admin.Name };
                applicationDbContext.Modules.Add(home);
                applicationDbContext.Modules.Add(administration);
                applicationDbContext.SaveChanges();

                var homeCategory = new Category { Name = "Home Category", Order = 1, Permission = user.Name };
                var authorisationCatgory = new Category { Name = "Authorisation", Order = 1, Permission = admin.Name };
                var configurationCategory = new Category { Name = "Configuration", Order = 2, Permission = admin.Name };
                applicationDbContext.Categories.Add(homeCategory);
                applicationDbContext.Categories.Add(authorisationCatgory);
                applicationDbContext.Categories.Add(configurationCategory);
                applicationDbContext.SaveChanges();

                home.Categories.Add(homeCategory);
                applicationDbContext.SaveChanges();

                administration.Categories.Add(authorisationCatgory);
                administration.Categories.Add(configurationCategory);
                applicationDbContext.SaveChanges();

                var homeMenuItem = new MenuItem { Name = "Home", ImageClass = "oi oi-home", NavigateTo = "/", Order = 1, Permission = user.Name, Config = "Home" };
                var usersMenuItem = new MenuItem { Name = "Users", ImageClass = "oi oi-person", NavigateTo = "Page", Order = 1, Permission = admin.Name, Config = "Users" };
                var rolesMenuItem = new MenuItem { Name = "Roles", ImageClass = "oi oi-lock-locked", NavigateTo = "Page", Order = 2, Permission = admin.Name, Config = "Roles" };
                var permissionsMenuItem = new MenuItem { Name = "Permissions", ImageClass = "oi oi-key", NavigateTo = "Page", Order = 3, Permission = admin.Name, Config = "Permissions" };
                var configureMenuItem = new MenuItem { Name = "Configure", ImageClass = "oi oi-cog", NavigateTo = "Page", Order = 1, Permission = admin.Name, Config = "Configs" };
                applicationDbContext.MenuItems.Add(homeMenuItem);
                applicationDbContext.MenuItems.Add(usersMenuItem);
                applicationDbContext.MenuItems.Add(rolesMenuItem);
                applicationDbContext.MenuItems.Add(permissionsMenuItem);
                applicationDbContext.MenuItems.Add(configureMenuItem);
                applicationDbContext.SaveChanges();

                homeCategory.MenuItems.Add(homeMenuItem);
                applicationDbContext.SaveChanges();

                authorisationCatgory.MenuItems.Add(usersMenuItem);
                authorisationCatgory.MenuItems.Add(rolesMenuItem);
                authorisationCatgory.MenuItems.Add(permissionsMenuItem);
                applicationDbContext.SaveChanges();

                configurationCategory.MenuItems.Add(configureMenuItem);
                applicationDbContext.SaveChanges();
            }

            if (!applicationDbContext.Configs.Any()
               && !applicationDbContext.ConfigItems.Any())
            {
                var permissionsConfig = new Config
                {
                    Name = "Permissions",
                    Title = "Permissions",
                    Model = "Headway.Core.Model.Permission, Headway.Core",
                    ModelApi = "Permissions",
                    OrderModelBy = "Name",
                    Container = "Headway.Razor.Controls.Containers.Table`1, Headway.Razor.Controls",
                    NavigateTo = "Page",
                    NavigateToProperty = "PermissionId",
                    NavigateToConfig = "Permission",
                    NavigateBack = "Page",
                    NavigateBackConfig = "Permissions"
                };

                var permissionConfig = new Config
                {
                    Name = "Permission",
                    Title = "Permission",
                    Model = "Headway.Core.Model.Permission, Headway.Core",
                    ModelApi = "Permissions",
                    Container = "Headway.Razor.Controls.Containers.Card`1, Headway.Razor.Controls",
                    NavigateTo = "Page",
                    NavigateToConfig = "Permissions",
                    NavigateBack = "Page",
                    NavigateBackConfig = "Permissions"
                };

                var configsConfig = new Config
                {
                    Name = "Configs",
                    Title = "Configs",
                    Model = "Headway.Core.Model.Config, Headway.Core",
                    ModelApi = "Configuration",
                    OrderModelBy = "Name",
                    Container = "Headway.Razor.Controls.Containers.Table`1, Headway.Razor.Controls",
                    NavigateTo = "Page",
                    NavigateToProperty = "ConfigId",
                    NavigateToConfig = "Config",
                    NavigateBack = "Page",
                    NavigateBackConfig = "Configs"
                };

                var configConfig = new Config
                {
                    Name = "Config",
                    Title = "Config",
                    Model = "Headway.Core.Model.Config, Headway.Core",
                    ModelApi = "Configuration",
                    Container = "Headway.Razor.Controls.Containers.Crud`1, Headway.Razor.Controls",
                    NavigateTo = "Page",
                    NavigateToConfig = "Configs",
                    NavigateBack = "Page",
                    NavigateBackConfig = "Configs"
                };

                var configItemsListDetailConfig = new Config
                {
                    Name = "ConfigItemsListDetail",
                    Title = "ConfigItemsListDetail",
                    Model = "Headway.Core.Model.ConfigItem, Headway.Core",
                    ModelApi = "Configuration",
                    OrderModelBy = "Order",
                    Container = "Headway.Razor.Controls.Containers.ListDetail`1, Headway.Razor.Controls"
                };

                var configItemConfig = new Config
                {
                    Name = "ConfigItem",
                    Title = "ConfigItem",
                    Model = "Headway.Core.Model.ConfigItem, Headway.Core",
                    ModelApi = "Configuration",
                    Container = "Headway.Razor.Controls.Components.ListDetail`1, Headway.Razor.Controls"
                };

                applicationDbContext.Configs.Add(configsConfig);
                applicationDbContext.Configs.Add(configConfig);
                applicationDbContext.Configs.Add(configItemsListDetailConfig);
                applicationDbContext.Configs.Add(configItemConfig);
                applicationDbContext.Configs.Add(permissionsConfig);
                applicationDbContext.Configs.Add(permissionConfig);
                applicationDbContext.SaveChanges();

                // Permissions /////////////////
                permissionsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PermissionId", Label = "Permission Id", Order = 1 });
                permissionsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", Order = 2 });
                permissionsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Description", Label = "Description", Order = 3 });
                applicationDbContext.SaveChanges();
                ////////////////////////////////

                // ConfigItem /////////////////
                var configItemConfigContainer = new ConfigContainer { Name = "Root Div", Container = "Headway.Razor.Controls.Containers.Div, Headway.Razor.Controls", Order = 1, IsRootContainer = true };
                configItemConfig.Containers.Add(configItemConfigContainer);
                applicationDbContext.SaveChanges();

                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ConfigItemId", Label = "Config Item Id", IsIdentity = true, Order = 1, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Label, Headway.Razor.Controls" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PropertyName", Label = "PropertyName", Order = 2, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ModelFieldsOptionItems|Name=Model;Value=Config" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Label", Label = "Label", Order = 3, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "IsIdentity", Label = "IsIdentity", Order = 4, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Checkbox, Headway.Razor.Controls" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "IsTitle", Label = "IsTitle", Order = 5, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Checkbox, Headway.Razor.Controls" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Order", Label = "Order", Order = 6, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Integer, Headway.Razor.Controls" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Component", Label = "Component", Order = 7, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ComponentOptionItems" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ComponentArgs", Label = "ComponentArgs", Order = 8, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ConfigName", Label = "Config Name", Order = 9, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ConfigOptionItems" });
                configItemConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ConfigContainer", Label = "Container", Order = 10, ConfigContainer = configItemConfigContainer, Component = "Headway.Razor.Controls.Components.GenericDropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ConfigContainers|Name=SearchParameter;Value=ConfigItem|Name=DisplayField;Value=Name|Name=Model;Value=Headway.Core.Model.ConfigContainer, Headway.Core|Name=Component;Value=Headway.Razor.Controls.Components.DropdownComplex`1, Headway.Razor.Controls" });
                applicationDbContext.SaveChanges();
                ////////////////////////////////

                // Config //////////////////////
                var configConfigContainer1 = new ConfigContainer { Name = "Tab Control", Container = "Headway.Razor.Controls.Containers.TabControl, Headway.Razor.Controls", Text = "Configure Model", Order = 1, IsRootContainer = true };
                var configConfigContainer2 = new ConfigContainer { Name = "Div", Container = "Headway.Razor.Controls.Containers.Div, Headway.Razor.Controls", Text = "Model", Order = 1 };
                var configConfigContainer3 = new ConfigContainer { Name = "Div", Container = "Headway.Razor.Controls.Containers.Div, Headway.Razor.Controls", Text = "Fields", Order = 1 };

                configConfigContainer1.ConfigContainers.Add(configConfigContainer2);
                configConfigContainer1.ConfigContainers.Add(configConfigContainer3);
                applicationDbContext.SaveChanges();

                configConfig.Containers.Add(configConfigContainer1);
                configConfig.Containers.Add(configConfigContainer2);
                configConfig.Containers.Add(configConfigContainer3);
                applicationDbContext.SaveChanges();

                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ConfigId", Label = "Config Id", IsIdentity = true, Order = 1, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Label, Headway.Razor.Controls" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", Order = 2, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Title", Label = "Title", IsTitle = true, Order = 3, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Model", Label = "Model", Order = 4, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ModelOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ModelApi", Label = "Model Api", Order = 5, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ControllerOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "OrderModelBy", Label = "Order Model By", Order = 6, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ModelFieldsOptionItems|Name=Model;Value=Config" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Container", Label = "Container", Order = 7, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ContainerOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "NavigateTo", Label = "Navigate To", Order = 8, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=PageOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "NavigateToProperty", Label = "Navigate To Property", Order = 9, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ModelFieldsOptionItems|Name=Model;Value=Config" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "NavigateToConfig", Label = "Navigate To Config", Order = 10, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ConfigOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "NavigateBack", Label = "Navigate Back", Order = 11, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=PageOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "NavigateBackProperty", Label = "Navigate Back Property", Order = 12, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ModelFieldsOptionItems|Name=Model;Value=Config" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "NavigateBackConfig", Label = "Navigate Back Config", Order = 13, ConfigContainer = configConfigContainer2, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = "Name=OptionsCode;Value=ConfigOptionItems" });
                configConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ConfigItems", Label = "Config Items", Order = 14, ConfigContainer = configConfigContainer3, Component = "Headway.Razor.Controls.Components.GenericList, Headway.Razor.Controls", ConfigName = "ConfigItem", ComponentArgs = "Name=ListConfig;Value=ConfigItemsListDetail" });
                applicationDbContext.SaveChanges();
                ////////////////////////////////

                // Configs //////////////////////
                configsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ConfigId", Label = "Config Id", Order = 1 });
                configsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", Order = 2 });
                configsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Title", Label = "Title", Order = 3 });
                configsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Model", Label = "Model", Order = 4 });
                applicationDbContext.SaveChanges();
                ////////////////////////////////

                // Config Items /////////////////
                configItemsListDetailConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PropertyName", Label = "Property Name", Order = 1 });
                configItemsListDetailConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Order", Label = "Order", Order = 2 });
                applicationDbContext.SaveChanges();
                ////////////////////////////////

                // Permissions//////////////////
                var permissionConfigContainer = new ConfigContainer { Name = "Root Div", Container = "Headway.Razor.Controls.Containers.Div, Headway.Razor.Controls", Order = 1 };
                permissionConfig.Containers.Add(permissionConfigContainer);
                applicationDbContext.SaveChanges();

                permissionConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PermissionId", Label = "Permission Id", IsIdentity = true, Order = 1, ConfigContainer = permissionConfigContainer, Component = "Headway.Razor.Controls.Components.Label, Headway.Razor.Controls" });
                permissionConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", IsTitle = true, Order = 2, ConfigContainer = permissionConfigContainer, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
                permissionConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Description", Label = "Description", Order = 3, ConfigContainer = permissionConfigContainer, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
                applicationDbContext.SaveChanges();
                ////////////////////////////////
            }
        }
    }
}
