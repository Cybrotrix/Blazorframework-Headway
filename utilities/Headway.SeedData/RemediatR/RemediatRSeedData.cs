﻿using Headway.Blazor.Controls.Flow;
using Headway.Core.Constants;
using Headway.Core.Model;
using Headway.Repository.Data;
using Microsoft.EntityFrameworkCore;
using RemediatR.Core.Constants;
using RemediatR.Core.Enums;
using RemediatR.Core.Model;
using RemediatR.Repository.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Headway.SeedData.RemediatR
{
    public class RemediatRSeedData
    {
        private static ApplicationDbContext dbContext;

        public static void Initialise(ApplicationDbContext applicationDbContext)
        {
            dbContext = applicationDbContext;

            TruncateTables();

            CreateCountries();
            CreatePermissions();
            CreateRoles();
            CreateUsers();
            AssignUsersRoles();
            CreateNavigation();
            CreatePrograms();
            CreateCustomers();

            ProgramsConfig();
            ProgramConfig();
            CustomersConfig();
            CustomerConfig();
            ProductConfig();
            ProductsListDetailConfig();
            RedressCasesConfig();
            NewRedressCasesConfig();
            RedressConfig();
            RedressCustomerConfig();
            RedressProductConfig();
            RefundCalculation();
            RefundVerification();
        }

        private static void TruncateTables()
        {
            dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE Countries");
            dbContext.Database.ExecuteSqlRaw("DELETE FROM Programs");
            dbContext.Database.ExecuteSqlRaw("DBCC CHECKIDENT (Programs, RESEED, 1)");
            dbContext.Database.ExecuteSqlRaw("DELETE FROM Products");
            dbContext.Database.ExecuteSqlRaw("DBCC CHECKIDENT (Products, RESEED, 1)");
            dbContext.Database.ExecuteSqlRaw("DELETE FROM Customers");
            dbContext.Database.ExecuteSqlRaw("DBCC CHECKIDENT (Customers, RESEED, 1)");
            dbContext.Database.ExecuteSqlRaw("DELETE FROM Redresses");
            dbContext.Database.ExecuteSqlRaw("DBCC CHECKIDENT (Redresses, RESEED, 1)");
        }

        private static void CreateCountries()
        {
            var countries = RemediatRData.CountriesCreate();

            foreach (var country in countries)
            {
                dbContext.Countries.Add(country);
            }
            
            dbContext.SaveChanges();
        }

        private static void CreatePermissions()
        {
            RemediatRData.PermissionsCreate();

            foreach (var permission in RemediatRData.Permissions.Values)
            {
                dbContext.Permissions.Add(permission);
            }

            dbContext.SaveChanges();
        }

        private static void CreateRoles()
        {
            RemediatRData.RolesCreate();

            foreach (var role in RemediatRData.Roles.Values)
            {
                dbContext.Roles.Add(role);
            }

            RemediatRData.RolesAssignPermissions();

            dbContext.SaveChanges();
        }

        private static void CreateUsers()
        {
            RemediatRData.UsersCreate();

            foreach (var user in RemediatRData.Users.Values)
            {
                dbContext.Users.Add(user);
            }

            dbContext.SaveChanges();
        }

        private static void AssignUsersRoles()
        {
            var userRole = dbContext.Roles
                .FirstOrDefault(r => r.Name.Equals(HeadwayAuthorisation.USER));

            foreach(var user in RemediatRData.Users.Values)
            {
                user.Roles.Add(userRole);
            }

            RemediatRData.UsersAssignRoles(assignAllUsersHeadwayUserRole: false);

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
            var redressCasesMenuItem = new MenuItem { Name = "Redress Cases", Icon = "List", NavigatePage = "Page", Order = 1, Permission = RemediatRAuthorisation.REDRESS_READ, Config = RemediatRSearchSource.REDRESSCASES };
            var createRedressCasesMenuItem = new MenuItem { Name = "New Redress Case", Icon = "PlaylistAdd", NavigatePage = "Page", Order = 1, Permission = RemediatRAuthorisation.REDRESS_READ, Config = RemediatRSearchSource.NEW_REDRESS_CASE };
            var programsMenuItem = new MenuItem { Name = "Programs", Icon = "AppRegistration", NavigatePage = "Page", Order = 1, Permission = HeadwayAuthorisation.ADMIN, Config = "Programs" };

            dbContext.MenuItems.Add(customersMenuItem);
            dbContext.MenuItems.Add(redressCasesMenuItem);
            dbContext.MenuItems.Add(createRedressCasesMenuItem);
            dbContext.MenuItems.Add(programsMenuItem);

            customerCatgory.MenuItems.Add(customersMenuItem);
            redressCatgory.MenuItems.Add(redressCasesMenuItem);
            redressCatgory.MenuItems.Add(createRedressCasesMenuItem);
            programCatgory.MenuItems.Add(programsMenuItem);

            remediatR.Categories.Add(customerCatgory);
            remediatR.Categories.Add(redressCatgory);
            remediatR.Categories.Add(programCatgory);

            dbContext.SaveChanges();
        }

        private static void CreatePrograms()
        {
            var programs = RemediatRData.ProgramsCreate();

            foreach (var program in programs)
            {
                dbContext.Programs.Add(program);
            }

            dbContext.SaveChanges();
        }

        private static void CreateCustomers()
        {
            var customers = RemediatRData.CustomersCreate();

            foreach (var customer in customers)
            {
                dbContext.Customers.Add(customer);
            }

            dbContext.SaveChanges();
        }

        private static void ProgramsConfig()
        {
            var programsConfig = RemediatRData.ProgramsConfigCreate();

            dbContext.Configs.Add(programsConfig);

            dbContext.SaveChanges();
        }

        private static void ProgramConfig()
        {
            var programConfig = RemediatRData.ProgramConfigCreate();

            dbContext.Configs.Add(programConfig);

            dbContext.SaveChanges();
        }

        private static void CustomersConfig()
        {
            var customersConfig = RemediatRData.CustomersConfigCreate();

            dbContext.Configs.Add(customersConfig);

            dbContext.SaveChanges();
        }

        private static void CustomerConfig()
        {
            var customerConfig = RemediatRData.CustomerConfigCreate();

            dbContext.Configs.Add(customerConfig);

            dbContext.SaveChanges();
        }

        private static void ProductConfig()
        {
            var productConfig = RemediatRData.ProductConfigCreate();

            dbContext.Configs.Add(productConfig);

            dbContext.SaveChanges();
        }

        private static void ProductsListDetailConfig()
        {
            var productsListDetailConfig = RemediatRData.ProductsListDetailConfigCreate();

            dbContext.Configs.Add(productsListDetailConfig);

            dbContext.SaveChanges();
        }

        private static void RedressCustomerConfig()
        {
            var redressCustomerConfig = RemediatRData.RedressCustomerConfigCreate();

            dbContext.Configs.Add(redressCustomerConfig);

            dbContext.SaveChanges();
        }

        private static void RedressProductConfig()
        {
            var redressProductConfig = new Config
            {
                Name = "RedressProduct",
                Title = "Product",
                Description = "Redress product sold to the customer",
                Model = "RemediatR.Core.Model.Product, RemediatR.Core",
                ModelApi = "RemediatRCustomer",
                Document = "Headway.Blazor.Controls.Documents.Content`1, Headway.Blazor.Controls"
            };

            dbContext.Configs.Add(redressProductConfig);

            var redressProductConfigContainer = new ConfigContainer { Name = "Redress Product Div", Code = "REDRESS PRODUCT DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Product", Order = 1 };

            redressProductConfig.ConfigContainers.Add(redressProductConfigContainer);

            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductId", Label = "Product Id", IsIdentity = true, Order = 1, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Name", Label = "Name", IsTitle = true, Order = 2, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.Text, Headway.Blazor.Controls" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Value", Label = "Value", Order = 3, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.FORMAT};Value={Args.FORMAT_F2}|Name={Args.MAX};Value=9999999999999.99" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Rate", Label = "Rate", Order = 4, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.FORMAT};Value={Args.FORMAT_F2}|Name={Args.MAX_LENGTH};Value=5|Name={Args.MAX};Value=99.99" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Duration", Label = "Duration", Order = 5, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.IntegerNullable, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.MIN};Value=3|Name={Args.MAX};Value=360", Tooltip = "Duration must be min 3 and max 360" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "StartDate", Label = "Start Date", Order = 6, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductType", Label = "ProductType", Order = 7, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.GenericDropdown, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.COMPONENT};Value=Headway.Blazor.Controls.Components.DropdownEnum`1, Headway.Blazor.Controls|Name={Args.MODEL};Value=RemediatR.Core.Enums.ProductType, RemediatR.Core" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RateType", Label = "RateType", Order = 8, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.GenericDropdown, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.COMPONENT};Value=Headway.Blazor.Controls.Components.DropdownEnum`1, Headway.Blazor.Controls|Name={Args.MODEL};Value=RemediatR.Core.Enums.RateType, RemediatR.Core" });
            redressProductConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RepaymentType", Label = "RepaymentType", Order = 9, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.GenericDropdown, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.COMPONENT};Value=Headway.Blazor.Controls.Components.DropdownEnum`1, Headway.Blazor.Controls|Name={Args.MODEL};Value=RemediatR.Core.Enums.RepaymentType, RemediatR.Core" });

            dbContext.SaveChanges();
        }

        private static void RedressCasesConfig()
        {
            var redressCasesConfig = new Config
            {
                Name = RemediatRSearchSource.REDRESSCASES,
                Title = "Redress Cases",
                Description = "List of RemediatR redress cases",
                Model = "RemediatR.Core.Model.RedressCase, RemediatR.Core",
                ModelApi = "RemediatRRedress",
                OrderModelBy = "CustomerName",
                Document = "Headway.Blazor.Controls.Documents.Table`1, Headway.Blazor.Controls",
                DocumentArgs = $"Name={Css.ROW_BTN_IMAGE};Value={Css.BTN_IMAGE_EDIT}|Name={Css.ROW_BTN_TOOLTIP};Value={Css.BTN_TOOLTIP_EDIT}",
                SearchComponent = "Headway.Blazor.Controls.SearchComponents.StandardSearchComponent, Headway.Blazor.Controls",
                UseSearchComponent = true,
                NavigatePage = "Page",
                NavigateProperty = "RedressId",
                NavigateConfig = "Redress",
                NavigateResetBreadcrumb = true
            };

            redressCasesConfig.ConfigSearchItems.AddRange(new List<ConfigSearchItem>
            {
                new ConfigSearchItem
                {
                    Label = "Program",
                    ParameterName = "Name",
                    Tooltip = "The redress program",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchDropdown, Headway.Blazor.Controls",
                    ComponentArgs = $"Name={Options.OPTIONS_CODE};Value={RemediatROptions.PROGRAMS_OPTION_ITEMS}|Name={Args.STYLE};Value=min-width:150px",
                    Order = 1
                },
                new ConfigSearchItem
                {
                    Label = "Redress Id",
                    ParameterName = "RedressId",
                    Tooltip = "The redress identifier",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchText, Headway.Blazor.Controls",
                    Order = 2
                },
                new ConfigSearchItem
                {
                    Label = "Customer Id",
                    ParameterName = "CustomerId",
                    Tooltip = "The customer identifier",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchText, Headway.Blazor.Controls",
                    Order = 3
                },
                new ConfigSearchItem
                {
                    Label = "Surname",
                    ParameterName = "Surname",
                    Tooltip = "The customer surname",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchText, Headway.Blazor.Controls",
                    Order = 4
                }
            });

            dbContext.Configs.Add(redressCasesConfig);

            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressId", Label = "Redress Id", Order = 1 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CustomerName", Label = "CustomerName", Order = 2 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProgramName", Label = "ProgramName", Order = 3 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductName", Label = "ProductName", Order = 4 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Status", Label = "Status", Order = 5 });

            dbContext.SaveChanges();
        }

        private static void NewRedressCasesConfig()
        {
            var redressCasesConfig = new Config
            {
                Name = RemediatRSearchSource.NEW_REDRESS_CASE,
                Title = "New Redress Case",
                Description = "Create a new RemediatR redress case",
                Model = "RemediatR.Core.Model.NewRedressCase, RemediatR.Core",
                ModelApi = "RemediatRRedress",
                OrderModelBy = "CustomerId",
                Document = "Headway.Blazor.Controls.Documents.Table`1, Headway.Blazor.Controls",
                DocumentArgs = $"Name={Css.ROW_BTN_IMAGE};Value={Css.BTN_IMAGE_PLUS}|Name={Css.ROW_BTN_TOOLTIP};Value={Css.BTN_TOOLTIP_CREATE}",
                SearchComponent = "Headway.Blazor.Controls.SearchComponents.StandardSearchComponent, Headway.Blazor.Controls",
                UseSearchComponent = true,
                NavigatePage = "Page",
                NavigateConfig = "Redress",
                NavigateResetBreadcrumb = true
            };

            redressCasesConfig.ConfigSearchItems.AddRange(new List<ConfigSearchItem>
            {
                new ConfigSearchItem
                {
                    Label = "Product Type",
                    ParameterName = "ProductType",
                    Tooltip = "The type of product in scope for redress",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchDropdown, Headway.Blazor.Controls",
                    ComponentArgs = $"Name={Options.OPTIONS_CODE};Value={Options.ENUM_NAMES_OPTION_ITEMS}|Name={Args.TYPE};Value=RemediatR.Core.Enums.ProductType, RemediatR.Core|Name={Args.STYLE};Value=min-width:150px",
                    Order = 1
                },
                new ConfigSearchItem
                {
                    Label = "Customer Id",
                    ParameterName = "CustomerId",
                    Tooltip = "The customer identifier",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchText, Headway.Blazor.Controls",
                    Order = 2
                },
                new ConfigSearchItem
                {
                    Label = "Surname",
                    ParameterName = "Surname",
                    Tooltip = "The customer surname",
                    Component = "Headway.Blazor.Controls.SearchComponents.SearchText, Headway.Blazor.Controls",
                    Order = 3
                }
            });

            dbContext.Configs.Add(redressCasesConfig);

            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressId", Label = "Redress Id", Order = 1 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProgramName", Label = "Program", Order = 2 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CustomerId", Label = "Customer Id", Order = 3 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CustomerName", Label = "Customer", Order = 4 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductId", Label = "Product Id", Order = 5 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductName", Label = "Product", Order = 4 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductType", Label = "Product Type", Order = 5 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RateType", Label = "Rate Type", Order = 6 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RepaymentType", Label = "Repayment Type", Order = 7 });
            redressCasesConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Status", Label = "Status", Order = 8 });

            dbContext.SaveChanges();
        }

        private static void RedressConfig()
        {
            var redressConfig = new Config
            {
                Name = "Redress",
                Title = "Redress",
                Description = "Manage a RemediatR redress case",
                Model = "RemediatR.Core.Model.Redress, RemediatR.Core",
                ModelApi = "RemediatRRedress",
                CreateLocal = false,
                Document = "Headway.Blazor.Controls.Documents.TabDocument`1, Headway.Blazor.Controls",
                NavigatePage = "Page",
                NavigateConfig = "RedressCases"
            };

            dbContext.Configs.Add(redressConfig);
            
            var redressConfigContainer = new ConfigContainer { Name = "Redress Div", Code = "REDRESS DIV", Container = "Headway.Blazor.Controls.Flow.Containers.FlowListContainer, Headway.Blazor.Controls", Label = "Redress Details", Order = 1, ComponentArgs = $"Name={FlowConstants.FLOW_LIST_CONTAINER_LABEL};Value=Redress Flow|Name={FlowConstants.FLOW_LIST_CONTAINER_WIDTH};Value=400px" };
            
            var redressDetailsContainer = new ConfigContainer { Name = "Redress Create Div", Code = "REDRESS CREATE DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Redress Create", Order = 1, ParentCode = "REDRESS DIV" };
            var refundAssessmentContainer = new ConfigContainer { Name = "Refund Assessment Div", Code = "REFUND ASSESSMENT DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Refund Assessment", Order = 2, ParentCode = "REDRESS DIV" };

            var refundCalculationContainer = new ConfigContainer { Name = "Refund Calculation Div", Code = "REFUND CALCULATION DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Refund Calculation", Order = 1, ParentCode = "REFUND ASSESSMENT DIV" };
            var refundVerificationContainer = new ConfigContainer { Name = "Refund Verification Div", Code = "REFUND VERIFICATION DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Refund Verification", Order = 2, ParentCode = "REFUND ASSESSMENT DIV" };

            var refundReviewContainer = new ConfigContainer { Name = "Refund Review Div", Code = "REFUND REVIEW DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Refund Review", Order = 3, ParentCode = "REDRESS DIV" };
            var redressReviewContainer = new ConfigContainer { Name = "Redress Review Div", Code = "REDRESS REVIEW DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Redress Review", Order = 4, ParentCode = "REDRESS DIV" };

            var redressValidationContainer = new ConfigContainer { Name = "Redress Validation Div", Code = "REDRESS VALIDATION DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Redress Validation Review", Order = 4, ParentCode = "REDRESS DIV" };
            var communicationGenerationContainer = new ConfigContainer { Name = "Communication Generation Div", Code = "COMMUNICATION GENERATION DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Communication Generation", Order = 5, ParentCode = "REDRESS DIV" };
            var communicationDispatchContainer = new ConfigContainer { Name = "Communication Dispatch Div", Code = "COMMUNICATION DISPATCH DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Communication Dispatch", Order = 6, ParentCode = "REDRESS DIV" };
            var responseRequiredContainer = new ConfigContainer { Name = "Response Required Div", Code = "RESPONSE REQUIRED DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Response Required", Order = 7, ParentCode = "REDRESS DIV" };

            var awaitingResponseContainer = new ConfigContainer { Name = "Awaiting Response Div", Code = "AWAITING RESPONSE DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Awaiting Response", Order = 8, ParentCode = "RESPONSE REQUIRED DIV" };

            var paymentGenerationContainer = new ConfigContainer { Name = "Payment Generation Div", Code = "PAYMENT GENERATION DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Payment Generation", Order = 9, ParentCode = "REDRESS DIV" };
            var finalRedressReviewContainer = new ConfigContainer { Name = "Final Redress Review Div", Code = "FINAL REDRESS REVIEW DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Final Redress Review", Order = 10, ParentCode = "REDRESS DIV" };

            var redressCustomerConfigContainer = new ConfigContainer { Name = "Redress Customer Tab Div", Code = "REDRESS CUSTOMER TAB DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Customer", Order = 2 };
            var redressProductConfigContainer = new ConfigContainer { Name = "Redress Product Tab Div", Code = "REDRESS PRODUCT TAB DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Product", Order = 3 };

            redressConfigContainer.ConfigContainers.Add(redressDetailsContainer);
            redressConfigContainer.ConfigContainers.Add(refundAssessmentContainer);
            redressConfigContainer.ConfigContainers.Add(refundReviewContainer);
            redressConfigContainer.ConfigContainers.Add(redressReviewContainer);
            redressConfigContainer.ConfigContainers.Add(redressValidationContainer);
            redressConfigContainer.ConfigContainers.Add(communicationGenerationContainer);
            redressConfigContainer.ConfigContainers.Add(communicationDispatchContainer);
            redressConfigContainer.ConfigContainers.Add(responseRequiredContainer);
            redressConfigContainer.ConfigContainers.Add(paymentGenerationContainer);
            redressConfigContainer.ConfigContainers.Add(finalRedressReviewContainer);

            refundAssessmentContainer.ConfigContainers.Add(refundCalculationContainer);
            refundAssessmentContainer.ConfigContainers.Add(refundVerificationContainer);

            responseRequiredContainer.ConfigContainers.Add(awaitingResponseContainer);

            redressConfig.ConfigContainers.Add(redressConfigContainer);
            redressConfig.ConfigContainers.Add(redressDetailsContainer);
            redressConfig.ConfigContainers.Add(refundAssessmentContainer);
            redressConfig.ConfigContainers.Add(refundCalculationContainer);
            redressConfig.ConfigContainers.Add(refundVerificationContainer);
            redressConfig.ConfigContainers.Add(refundReviewContainer);
            redressConfig.ConfigContainers.Add(redressReviewContainer);
            redressConfig.ConfigContainers.Add(redressValidationContainer);
            redressConfig.ConfigContainers.Add(communicationGenerationContainer);
            redressConfig.ConfigContainers.Add(communicationDispatchContainer);
            redressConfig.ConfigContainers.Add(responseRequiredContainer);
            redressConfig.ConfigContainers.Add(awaitingResponseContainer);
            redressConfig.ConfigContainers.Add(paymentGenerationContainer);
            redressConfig.ConfigContainers.Add(finalRedressReviewContainer);

            redressConfig.ConfigContainers.Add(redressCustomerConfigContainer);
            redressConfig.ConfigContainers.Add(redressProductConfigContainer);

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressId", Label = "Redress Id", IsIdentity = true, Order = 1, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Program", Label = "Program", Order = 2, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.GenericDropdown, Headway.Blazor.Controls", ComponentArgs = $"Name={Options.OPTIONS_CODE};Value={RemediatROptions.PROGRAMS_COMPLEX_OPTION_ITEMS}|Name={Options.DISPLAY_FIELD};Value=Name|Name={Args.MODEL};Value=RemediatR.Core.Model.Program, RemediatR.Core|Name={Args.COMPONENT};Value=Headway.Blazor.Controls.Components.DropdownComplex`1, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CustomerName", Label = "Customer", IsTitle = true, Order = 3, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ProductName", Label = "Product", IsTitle = false, Order = 4, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressCaseOwner", Label = "Redress Case Owner", IsTitle = false, Order = 5, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressCreateBy", Label = "Redress Create By", IsTitle = false, Order = 6, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressCreateDate", Label = "Redress Create Date", IsTitle = false, Order = 7, ConfigContainer = redressDetailsContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundAssessmentStatus", Label = "Refund Review Status", IsTitle = false, Order = 8, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundAssessmentBy", Label = "Refund Review By", IsTitle = false, Order = 9, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundAssessmentDate", Label = "Refund Review Date", IsTitle = false, Order = 10, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundCalculation", Label = "Refund Calculation", Order = 11, ConfigContainer = refundCalculationContainer, Component = "Headway.Blazor.Controls.Components.GenericField, Headway.Blazor.Controls", ConfigName = "RefundCalculation" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundCalculation", Label = "Refund Verification", Order = 12, ConfigContainer = refundVerificationContainer, Component = "Headway.Blazor.Controls.Components.GenericField, Headway.Blazor.Controls", ConfigName = "RefundVerification" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundReviewStatus", Label = "Refund Review Status", IsTitle = false, Order = 13, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundReviewComment", Label = "Refund Review Comment", IsTitle = false, Order = 14, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.TextMultiline, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.TEXT_MULTILINE_ROWS};Value=3" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundReviewBy", Label = "Refund Review By", IsTitle = false, Order = 15, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundReviewDate", Label = "Refund Review Date", IsTitle = false, Order = 16, ConfigContainer = refundReviewContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });
            
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressReviewStatus", Label = "Redress Review Status", IsTitle = false, Order = 17, ConfigContainer = redressReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressReviewComment", Label = "Redress Review Comment", IsTitle = false, Order = 18, ConfigContainer = redressReviewContainer, Component = "Headway.Blazor.Controls.Components.TextMultiline, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.TEXT_MULTILINE_ROWS};Value=3" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressReviewBy", Label = "Redress Review By", IsTitle = false, Order = 19, ConfigContainer = redressReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressReviewDate", Label = "Redress Review Date", IsTitle = false, Order = 20, ConfigContainer = redressReviewContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressValidationStatus", Label = "Redress Validation Status", IsTitle = false, Order = 21, ConfigContainer = redressValidationContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressValidationComment", Label = "Redress Validation Comment", IsTitle = false, Order = 22, ConfigContainer = redressValidationContainer, Component = "Headway.Blazor.Controls.Components.TextMultiline, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.TEXT_MULTILINE_ROWS};Value=3" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressValidationBy", Label = "Redress Validation By", IsTitle = false, Order = 23, ConfigContainer = redressValidationContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RedressValidationDate", Label = "Redress Validation Date", IsTitle = false, Order = 24, ConfigContainer = redressValidationContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationGenerationStatus", Label = "Communication Generation Status", IsTitle = false, Order = 25, ConfigContainer = communicationGenerationContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationGenerationBy", Label = "Communication Generation By", IsTitle = false, Order = 26, ConfigContainer = communicationGenerationContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationGenerationDate", Label = "Communication Generation Date", IsTitle = false, Order = 27, ConfigContainer = communicationGenerationContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationDispatchStatus", Label = "Communication Dispatch Status", IsTitle = false, Order = 28, ConfigContainer = communicationDispatchContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationDispatchComment", Label = "Communication Dispatch Comment", IsTitle = false, Order = 29, ConfigContainer = communicationDispatchContainer, Component = "Headway.Blazor.Controls.Components.TextMultiline, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.TEXT_MULTILINE_ROWS};Value=3" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationDispatchBy", Label = "Communication Dispatch By", IsTitle = false, Order = 30, ConfigContainer = communicationDispatchContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CommunicationDispatchDate", Label = "Communication Dispatch Date", IsTitle = false, Order = 31, ConfigContainer = communicationDispatchContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ResponseRequired", Label = "Response Required", IsTitle = false, Order = 32, ConfigContainer = responseRequiredContainer, Component = "Headway.Blazor.Controls.Components.CheckboxNullable, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "ResponseReceived", Label = "Response Received", IsTitle = false, Order = 33, ConfigContainer = responseRequiredContainer, Component = "Headway.Blazor.Controls.Components.CheckboxNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "AwaitingResponseStatus", Label = "Awaiting Response Status", IsTitle = false, Order = 34, ConfigContainer = awaitingResponseContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "AwaitingResponseComment", Label = "Awaiting Response Comment", IsTitle = false, Order = 35, ConfigContainer = awaitingResponseContainer, Component = "Headway.Blazor.Controls.Components.TextMultiline, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.TEXT_MULTILINE_ROWS};Value=3" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "AwaitingResponseBy", Label = "Awaiting Response By", IsTitle = false, Order = 36, ConfigContainer = awaitingResponseContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "AwaitingResponseDate", Label = "Awaiting Response Date", IsTitle = false, Order = 37, ConfigContainer = awaitingResponseContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PaymentGenerationStatus", Label = "Payment Generation Status", IsTitle = false, Order = 38, ConfigContainer = paymentGenerationContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PaymentGenerationBy", Label = "Payment Generation By", IsTitle = false, Order = 39, ConfigContainer = paymentGenerationContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "PaymentGenerationDate", Label = "Payment Generation Date", IsTitle = false, Order = 40, ConfigContainer = paymentGenerationContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "FinalRedressReviewStatus", Label = "Final Redress Review Status", IsTitle = false, Order = 41, ConfigContainer = finalRedressReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "FinalRedressReviewComment", Label = "Final Redress Review Comment", IsTitle = false, Order = 42, ConfigContainer = finalRedressReviewContainer, Component = "Headway.Blazor.Controls.Components.TextMultiline, Headway.Blazor.Controls", ComponentArgs = $"Name={Args.TEXT_MULTILINE_ROWS};Value=3" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "FinalRedressReviewBy", Label = "Final Redress Review By", IsTitle = false, Order = 43, ConfigContainer = finalRedressReviewContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "FinalRedressReviewDate", Label = "Final Redress Review Date", IsTitle = false, Order = 44, ConfigContainer = finalRedressReviewContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Customer", Label = "Customer", Order = 45, ConfigContainer = redressCustomerConfigContainer, Component = "Headway.Blazor.Controls.Components.GenericField, Headway.Blazor.Controls", ConfigName = "RedressCustomer" });
            redressConfig.ConfigItems.Add(new ConfigItem { PropertyName = "Product", Label = "Product", Order = 46, ConfigContainer = redressProductConfigContainer, Component = "Headway.Blazor.Controls.Components.GenericField, Headway.Blazor.Controls", ConfigName = "RedressProduct" });

            dbContext.SaveChanges();
        }

        private static void RefundCalculation()
        {
            var refundCalculationConfig = new Config
            {
                Name = "RefundCalculation",
                Title = "Refund Calculation",
                Description = "Manage a RemediatR refund calculation",
                Model = "RemediatR.Core.Model.RefundCalculation, RemediatR.Core",
                ModelApi = "RemediatRRedress",
                Document = "Headway.Blazor.Controls.Documents.Content`1, Headway.Blazor.Controls"
            };

            dbContext.Configs.Add(refundCalculationConfig);

            var refundCalculationConfigContainer = new ConfigContainer { Name = "Refund Calculation Fields Div", Code = "REFUND CALCULATION FIELD DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Refund Calculation", Order = 1 };

            refundCalculationConfig.ConfigContainers.Add(refundCalculationConfigContainer);

            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundCalculationId", Label = "Refund Calculation Id", IsIdentity = true, Order = 1, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "BasicRefundAmount", Label = "Basic Refund Amount", Order = 2, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CompensatoryAmount", Label = "Compensatory Amount", IsTitle = true, Order = 3, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CompensatoryInterestAmount", Label = "Compensatory Interest Amount", IsTitle = false, Order = 4, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "TotalCompensatoryAmount", Label = "Total Compensatory Amount", IsTitle = false, Order = 5, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "TotalRefundAmount", Label = "Total Refund Amount", IsTitle = false, Order = 6, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CalculatedBy", Label = "Calculated By", IsTitle = false, Order = 7, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.Text, Headway.Blazor.Controls" });
            refundCalculationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "CalculatedDate", Label = "Calculated Date", IsTitle = false, Order = 8, ConfigContainer = refundCalculationConfigContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            dbContext.SaveChanges();
        }

        private static void RefundVerification()
        {
            var refundVerificationConfig = new Config
            {
                Name = "RefundVerification",
                Title = "Refund Verification",
                Description = "Manage a RemediatR refund verification",
                Model = "RemediatR.Core.Model.RefundCalculation, RemediatR.Core",
                ModelApi = "RemediatRRedress",
                Document = "Headway.Blazor.Controls.Documents.Content`1, Headway.Blazor.Controls"
            };

            dbContext.Configs.Add(refundVerificationConfig);

            var refundVerificationConfigContainer = new ConfigContainer { Name = "Refund Verification Fields Div", Code = "REFUND VERIFICATION FIELD DIV", Container = "Headway.Blazor.Controls.Containers.Div, Headway.Blazor.Controls", Label = "Refund Verification", Order = 1 };

            refundVerificationConfig.ConfigContainers.Add(refundVerificationConfigContainer);

            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "RefundCalculationId", Label = "Refund Calculation Id", IsIdentity = true, Order = 1, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.Label, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedBasicRefundAmount", Label = "Verified Basic Refund Amount", Order = 2, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedCompensatoryAmount", Label = "Verified Compensatory Amount", IsTitle = true, Order = 3, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedCompensatoryInterestAmount", Label = "Verified Compensatory Interest Amount", IsTitle = false, Order = 4, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedTotalCompensatoryAmount", Label = "Verified Total Compensatory Amount", IsTitle = false, Order = 5, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedTotalRefundAmount", Label = " Total Refund Amount", IsTitle = false, Order = 6, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.DecimalNullable, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedBy", Label = "Verified By", IsTitle = false, Order = 7, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.Text, Headway.Blazor.Controls" });
            refundVerificationConfig.ConfigItems.Add(new ConfigItem { PropertyName = "VerifiedDate", Label = "Verified Date", IsTitle = false, Order = 8, ConfigContainer = refundVerificationConfigContainer, Component = "Headway.Blazor.Controls.Components.DateNullable, Headway.Blazor.Controls" });

            dbContext.SaveChanges();
        }
    }
}
