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
    public partial class Students
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

        protected IEnumerable<StudentMAUI.Models.ConData.Student> students;

        protected RadzenDataGrid<StudentMAUI.Models.ConData.Student> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            students = await ConDataService.GetStudents(new Query { Filter = $@"i => i.FirstName.Contains(@0) || i.LastName.Contains(@0) || i.EmailAddress.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Gender" });
        }
        protected override async Task OnInitializedAsync()
        {
            students = await ConDataService.GetStudents(new Query { Filter = $@"i => i.FirstName.Contains(@0) || i.LastName.Contains(@0) || i.EmailAddress.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Gender" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddStudent>("Add Student", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<StudentMAUI.Models.ConData.Student> args)
        {
            await DialogService.OpenAsync<EditStudent>("Edit Student", new Dictionary<string, object> { {"StudentID", args.Data.StudentID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, StudentMAUI.Models.ConData.Student student)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await ConDataService.DeleteStudent(student.StudentID);

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
                    Detail = $"Unable to delete Student" 
                });
            }
        }
    }
}