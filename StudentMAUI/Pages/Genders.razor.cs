using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace StudentMAUI.Pages
{
    public partial class Genders
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

        protected IEnumerable<StudentMAUI.Models.ConData.Gender> genders;

        protected RadzenDataGrid<StudentMAUI.Models.ConData.Gender> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            genders = await ConDataService.GetGenders(new Query { Filter = $@"i => i.GenderName.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            genders = await ConDataService.GetGenders(new Query { Filter = $@"i => i.GenderName.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddGender>("Add Gender", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<StudentMAUI.Models.ConData.Gender> args)
        {
            await DialogService.OpenAsync<EditGender>("Edit Gender", new Dictionary<string, object> { {"GenderID", args.Data.GenderID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, StudentMAUI.Models.ConData.Gender gender)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await ConDataService.DeleteGender(gender.GenderID);

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
                    Detail = $"Unable to delete Gender" 
                });
            }
        }
    }
}