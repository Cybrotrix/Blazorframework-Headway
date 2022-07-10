﻿using Headway.Core.Constants;
using Headway.Core.Model;
using Headway.RemediatR.Core.Constants;
using Headway.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Headway.SeedData
{
    public class RemediatRData
    {
        private static ApplicationDbContext dbContext;

        private static Dictionary<string, Permission> permissions = new Dictionary<string, Permission>();
        private static Dictionary<string, Role> roles = new Dictionary<string, Role>();
        private static Dictionary<string, User> users = new Dictionary<string, User>();

        public static void Initialise(ApplicationDbContext applicationDbContext)
        {
            dbContext = applicationDbContext;

            TruncateTables();

            CreatePermissions();
            CreateRoles();
            CreateUsers();
            AssignUsersRoles();
            CreateNavigation();

            ProgramsConfig();
            ProgramConfig();
        }

        private static void TruncateTables()
        {
            ((DbContext)dbContext).Database.ExecuteSqlRaw("DELETE FROM Programs");
            ((DbContext)dbContext).Database.ExecuteSqlRaw("DBCC CHECKIDENT (Programs, RESEED, 1)");
            //((DbContext)dbContext).Database.ExecuteSqlRaw("TRUNCATE TABLE PermissionUser");
            //((DbContext)dbContext).Database.ExecuteSqlRaw("TRUNCATE TABLE PermissionRole");
            //((DbContext)dbContext).Database.ExecuteSqlRaw("DELETE FROM Users");
            //((DbContext)dbContext).Database.ExecuteSqlRaw("DBCC CHECKIDENT (Users, RESEED, 1)");
        }

        private static void CreatePermissions()
        {
            permissions.Add(RemediatRAuthorisation.CUSTOMER_READ, new Permission { Name = RemediatRAuthorisation.CUSTOMER_READ, Description = "RemediatR Customer Read" });
            permissions.Add(RemediatRAuthorisation.CUSTOMER_WRITE, new Permission { Name = RemediatRAuthorisation.CUSTOMER_WRITE, Description = "RemediatR Customer Write" });

            permissions.Add(RemediatRAuthorisation.REDRESS_READ, new Permission { Name = RemediatRAuthorisation.REDRESS_READ, Description = "RemediatR Redress Read" });
            permissions.Add(RemediatRAuthorisation.REDRESS_WRITE, new Permission { Name = RemediatRAuthorisation.REDRESS_WRITE, Description = "RemediatR Redress Write" });
            permissions.Add(RemediatRAuthorisation.REDRESS_TRANSITION, new Permission { Name = RemediatRAuthorisation.REDRESS_TRANSITION, Description = "RemediatR Redress Transition" });
            permissions.Add(RemediatRAuthorisation.COMMUNICATION_DISPATCH_TRANSITION, new Permission { Name = RemediatRAuthorisation.COMMUNICATION_DISPATCH_TRANSITION, Description = "RemediatR Communication Dispatch Transition" });
            permissions.Add(RemediatRAuthorisation.AWAITING_REPONSE_TRANSITION, new Permission { Name = RemediatRAuthorisation.AWAITING_REPONSE_TRANSITION, Description = "RemediatR Awaiting Response Transition" });

            permissions.Add(RemediatRAuthorisation.REDRESS_REVIEW_TRANSITION, new Permission { Name = RemediatRAuthorisation.REDRESS_REVIEW_TRANSITION, Description = "RemediatR Redress Refund Review Transition" });
            permissions.Add(RemediatRAuthorisation.REDRESS_COMPLETE, new Permission { Name = RemediatRAuthorisation.REDRESS_COMPLETE, Description = "RemediatR Redress Complete" });

            permissions.Add(RemediatRAuthorisation.REFUND_READ, new Permission { Name = RemediatRAuthorisation.REFUND_READ, Description = "RemediatR Refund Read" });
            permissions.Add(RemediatRAuthorisation.REFUND_WRITE, new Permission { Name = RemediatRAuthorisation.REFUND_WRITE, Description = "RemediatR Refund Write" });
            permissions.Add(RemediatRAuthorisation.REFUND_CACULATION_COMPLETE, new Permission { Name = RemediatRAuthorisation.REFUND_CACULATION_COMPLETE, Description = "RemediatR Refund Calculation Complete" });
            permissions.Add(RemediatRAuthorisation.REFUND_VERIFICATION_COMPLETE, new Permission { Name = RemediatRAuthorisation.REFUND_VERIFICATION_COMPLETE, Description = "RemediatR Refund Varification Complete" });

            permissions.Add(RemediatRAuthorisation.REFUND_REVIEW_TRANSITION, new Permission { Name = RemediatRAuthorisation.REFUND_REVIEW_TRANSITION, Description = "RemediatR Refund Review Transition" });

            foreach(var permission in permissions.Values)
            {
                dbContext.Permissions.Add(permission);
            }

            dbContext.SaveChanges();
        }

        private static void CreateRoles()
        {
            roles.Add(RemediatRAuthorisation.REDRESS_CASE_OWNER, new Role { Name = RemediatRAuthorisation.REDRESS_CASE_OWNER, Description = "RemediatR Redress Case Owner" });
            roles.Add(RemediatRAuthorisation.REDRESS_REVIEWER, new Role { Name = RemediatRAuthorisation.REDRESS_REVIEWER, Description = "RemediatR Redress Reviewer" });
            roles.Add(RemediatRAuthorisation.REFUND_ASSESSOR, new Role { Name = RemediatRAuthorisation.REFUND_ASSESSOR, Description = "RemediatR Refund Assessor" });
            roles.Add(RemediatRAuthorisation.REFUND_REVIEWER, new Role { Name = RemediatRAuthorisation.REFUND_REVIEWER, Description = "RemediatR Refund Reviewer" });

            foreach (var role in roles.Values)
            {
                dbContext.Roles.Add(role);
            }

            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.CUSTOMER_READ]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.CUSTOMER_WRITE]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_READ]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_WRITE]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_TRANSITION]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_READ]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.COMMUNICATION_DISPATCH_TRANSITION]);
            roles[RemediatRAuthorisation.REDRESS_CASE_OWNER].Permissions.Add(permissions[RemediatRAuthorisation.AWAITING_REPONSE_TRANSITION]);

            roles[RemediatRAuthorisation.REDRESS_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.CUSTOMER_READ]);
            roles[RemediatRAuthorisation.REDRESS_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_READ]);
            roles[RemediatRAuthorisation.REDRESS_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_READ]);
            roles[RemediatRAuthorisation.REDRESS_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_REVIEW_TRANSITION]);
            roles[RemediatRAuthorisation.REDRESS_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_COMPLETE]);

            roles[RemediatRAuthorisation.REFUND_ASSESSOR].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_READ]);
            roles[RemediatRAuthorisation.REFUND_ASSESSOR].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_WRITE]);
            roles[RemediatRAuthorisation.REFUND_ASSESSOR].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_READ]);
            roles[RemediatRAuthorisation.REFUND_ASSESSOR].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_CACULATION_COMPLETE]);
            roles[RemediatRAuthorisation.REFUND_ASSESSOR].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_VERIFICATION_COMPLETE]);

            roles[RemediatRAuthorisation.REFUND_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_READ]);
            roles[RemediatRAuthorisation.REFUND_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REDRESS_READ]);
            roles[RemediatRAuthorisation.REFUND_REVIEWER].Permissions.Add(permissions[RemediatRAuthorisation.REFUND_REVIEW_TRANSITION]);

            dbContext.SaveChanges();
        }

        private static void CreateUsers()
        {
            users.Add("grace", new User { UserName = "grace", Email = "grace@email.com" });
            users.Add("mel", new User { UserName = "mel", Email = "mel@email.com" });
            users.Add("bill", new User { UserName = "bill", Email = "bill@email.com" });
            users.Add("will", new User { UserName = "will", Email = "will@email.com" });
            users.Add("mary", new User { UserName = "mary", Email = "mary@email.com" });

            foreach (var user in users.Values)
            {
                dbContext.Users.Add(user);
            }

            dbContext.SaveChanges();
        }

        private static void AssignUsersRoles()
        {
            users["grace"].Roles.Add(roles[RemediatRAuthorisation.REDRESS_CASE_OWNER]);
            users["mel"].Roles.Add(roles[RemediatRAuthorisation.REDRESS_REVIEWER]);
            users["bill"].Roles.Add(roles[RemediatRAuthorisation.REFUND_ASSESSOR]);
            users["will"].Roles.Add(roles[RemediatRAuthorisation.REFUND_ASSESSOR]);
            users["mary"].Roles.Add(roles[RemediatRAuthorisation.REFUND_REVIEWER]);

            dbContext.SaveChanges();
        }

        private static void CreateNavigation()
        {
            var remediatR = new Module { Name = "RemediatR", Icon = "Balance", Order = 1, Permission = RemediatRAuthorisation.REDRESS_READ };

            dbContext.Modules.Add(remediatR);

            var customerCatgory = new Category { Name = "Customer", Icon = "PersonOutline", Order = 1, Permission = RemediatRAuthorisation.CUSTOMER_READ };
            var redressCatgory = new Category { Name = "Redress", Icon = "ListAlt", Order = 2, Permission = RemediatRAuthorisation.REDRESS_READ };
            var programCatgory = new Category { Name = "Program", Icon = "Apps", Order = 3, Permission = HeadwayAuthorisation.ADMIN };

            dbContext.Categories.Add(customerCatgory);
            dbContext.Categories.Add(redressCatgory);
            dbContext.Categories.Add(programCatgory);

            var customersMenuItem = new MenuItem { Name = "Customers", Icon = "PeopleOutline", NavigatePage = "Page", Order = 1, Permission = RemediatRAuthorisation.CUSTOMER_READ, Config = "Customers" };
            var redressCasesMenuItem = new MenuItem { Name = "Redress Cases", Icon = "List", NavigatePage = "Page", Order = 1, Permission = RemediatRAuthorisation.REDRESS_READ, Config = "Redress Cases" };
            var programsMenuItem = new MenuItem { Name = "Programs", Icon = "AppRegistration", NavigatePage = "Page", Order = 1, Permission = HeadwayAuthorisation.ADMIN, Config = "Programs" };

            dbContext.MenuItems.Add(customersMenuItem);
            dbContext.MenuItems.Add(redressCasesMenuItem);
            dbContext.MenuItems.Add(programsMenuItem);

            customerCatgory.MenuItems.Add(customersMenuItem);
            redressCatgory.MenuItems.Add(redressCasesMenuItem);
            programCatgory.MenuItems.Add(programsMenuItem);

            remediatR.Categories.Add(customerCatgory);
            remediatR.Categories.Add(redressCatgory);
            remediatR.Categories.Add(programCatgory);

            dbContext.SaveChanges();
        }

        private static void ProgramsConfig()
        {
            var programsConfig = new Config
            {
                Name = "Programs",
                Title = "Programs",
                Description = "List of RemediatR programs",
                Model = "Headway.RemediatR.Core.Model.Program, Headway.RemediatR.Core",
                ModelApi = "RemediatRProgram",
                OrderModelBy = "Name",
                Document = "Headway.Razor.Controls.Documents.Table`1, Headway.Razor.Controls",
                NavigatePage = "Page",
                NavigateProperty = "ProgramId",
                NavigateConfig = "Program",
                NavigateResetBreadcrumb = true
            };

            dbContext.Configs.Add(programsConfig);

            programsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProgramId", Label = "Program Id", Order = 1 });
            programsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", Order = 2 });
            programsConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Description", Label = "Description", Order = 3 });

            dbContext.SaveChanges();
        }

        private static void ProgramConfig()
        {
            var programConfig = new Config
            {
                Name = "Program",
                Title = "Program",
                Description = "Create, update or delete a User Permission",
                Model = "Headway.RemediatR.Core.Model.Program, Headway.RemediatR.Core",
                ModelApi = "RemediatRProgram",
                CreateLocal = true,
                Document = "Headway.Razor.Controls.Documents.TabDocument`1, Headway.Razor.Controls",
                NavigatePage = "Page",
                NavigateConfig = "Programs"
            };

            dbContext.Configs.Add(programConfig);

            var programConfigContainer = new ConfigContainer { Name = "Program Div", Code = "PROGRAM DIV", Container = "Headway.Razor.Controls.Containers.Div, Headway.Razor.Controls", Label = "Program", Order = 1 };

            programConfig.ConfigContainers.Add(programConfigContainer);

            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProgramId", Label = "Program Id", IsIdentity = true, Order = 1, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.Label, Headway.Razor.Controls" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", IsTitle = true, Order = 2, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Description", Label = "Description", Order = 3, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.Text, Headway.Razor.Controls" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Compensation", Label = "Compensation", Order = 4, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.DecimalNullable, Headway.Razor.Controls", ComponentArgs = $"Name={Args.FORMAT};Value={Args.FORMAT_F2}|Name={Args.MAX};Value=9999999999999.99" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CompensatoryInterest", Label = "Compensatory Interest", Order = 5, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.DecimalNullable, Headway.Razor.Controls", ComponentArgs = $"Name={Args.FORMAT};Value={Args.FORMAT_F2}|Name={Args.MAX_LENGTH};Value=5|Name={Args.MAX};Value=99.99" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "StartDate", Label = "Start Date", Order = 6, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.DateNullable, Headway.Razor.Controls" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "EndDate", Label = "End Date", Order = 7, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.DateNullable, Headway.Razor.Controls" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductType", Label = "ProductType", Order = 8, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = $"Name={Options.OPTIONS_CODE};Value={Options.ENUM_OPTION_ITEMS}|Name={Args.ENUM};Value=Headway.RemediatR.Core.Enums.ProductType, Headway.RemediatR.Core" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RateType", Label = "RateType", Order = 9, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = $"Name={Options.OPTIONS_CODE};Value={Options.ENUM_OPTION_ITEMS}|Name={Args.ENUM};Value=Headway.RemediatR.Core.Enums.RateType, Headway.RemediatR.Core" });
            programConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RepaymentType", Label = "RepaymentType", Order = 10, ConfigContainer = programConfigContainer, Component = "Headway.Razor.Controls.Components.Dropdown, Headway.Razor.Controls", ComponentArgs = $"Name={Options.OPTIONS_CODE};Value={Options.ENUM_OPTION_ITEMS}|Name={Args.ENUM};Value=Headway.RemediatR.Core.Enums.RepaymentType, Headway.RemediatR.Core" });

            dbContext.SaveChanges();
        }
    }
}
