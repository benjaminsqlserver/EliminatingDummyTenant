using System.Net.Http;
using AzureADTenantPart2.Models.ConData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace AzureADTenantPart2.Pages
{
    public partial class Index
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        //Overriding OnInitializedAsync method in code behind of index page which is our start page
        protected override async Task OnInitializedAsync()
        {
            //get application current URL by accessing the BaseURI property of Navigation manager
            var appURL = NavigationManager.BaseUri;
            //call UserIsAccessingAssignedURL method of Security service
            bool isRightURL = await Security.UserIsAccessingAssignedURL(NavigationManager);
            //If method returns false redirect user to unauthorized page
            if (isRightURL == false)
            {
                NavigationManager.NavigateTo("unauthorized", true);
            }
        }
    }
}