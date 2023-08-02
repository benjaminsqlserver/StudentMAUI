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
    public partial class AddStudent
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

        protected override async Task OnInitializedAsync()
        {
            student = new StudentMAUI.Models.ConData.Student();

            gendersForGenderID = await ConDataService.GetGenders();
        }
        protected bool errorVisible;
        protected StudentMAUI.Models.ConData.Student student;

        protected IEnumerable<StudentMAUI.Models.ConData.Gender> gendersForGenderID;

        protected async Task FormSubmit()
        {
            try
            {
                await ConDataService.CreateStudent(student);
                DialogService.Close(student);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}