using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace AzureADTenantPart2.Pages
{
    public partial class Offices
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
        public ConDataService ConDataService { get; set; }

        protected IEnumerable<AzureADTenantPart2.Models.ConData.Office> offices;

        protected RadzenDataGrid<AzureADTenantPart2.Models.ConData.Office> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            ConDataService = await Security.CreateDataServiceForUser(NavigationManager);

            offices = await ConDataService.GetOffices(new Query { Filter = $@"i => i.OfficeName.Contains(@0) || i.OfficeAddress.Contains(@0) || i.Email.Contains(@0) || i.TelephoneNumber.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            bool isRightURL = await Security.UserIsAccessingAssignedURL(NavigationManager);
            //If method returns false redirect user to unauthorized page
            if (isRightURL == false)
            {
                NavigationManager.NavigateTo("unauthorized", true);
            }
            ConDataService = await Security.CreateDataServiceForUser(NavigationManager);
            offices = await ConDataService.GetOffices(new Query { Filter = $@"i => i.OfficeName.Contains(@0) || i.OfficeAddress.Contains(@0) || i.Email.Contains(@0) || i.TelephoneNumber.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddOffice>("Add Office", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<AzureADTenantPart2.Models.ConData.Office> args)
        {
            await DialogService.OpenAsync<EditOffice>("Edit Office", new Dictionary<string, object> { {"OfficeID", args.Data.OfficeID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AzureADTenantPart2.Models.ConData.Office office)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await ConDataService.DeleteOffice(office.OfficeID);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                { 
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error", 
                    Detail = $"Unable to delete Office" 
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await ConDataService.ExportOfficesToCSV(new Query
{ 
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", 
    OrderBy = $"{grid0.Query.OrderBy}", 
    Expand = "", 
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Offices");
            }

            if (args == null || args.Value == "xlsx")
            {
                await ConDataService.ExportOfficesToExcel(new Query
{ 
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", 
    OrderBy = $"{grid0.Query.OrderBy}", 
    Expand = "", 
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Offices");
            }
        }
    }
}