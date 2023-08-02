using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using StudentMAUI.Data;

namespace StudentMAUI
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ConDataContext context;
        private readonly NavigationManager navigationManager;

        public ConDataService(ConDataContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportGendersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/genders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/genders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGendersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/genders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/genders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGendersRead(ref IQueryable<StudentMAUI.Models.ConData.Gender> items);

        public async Task<IQueryable<StudentMAUI.Models.ConData.Gender>> GetGenders(Query query = null)
        {
            var items = Context.Genders.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnGendersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGenderGet(StudentMAUI.Models.ConData.Gender item);
        partial void OnGetGenderByGenderId(ref IQueryable<StudentMAUI.Models.ConData.Gender> items);


        public async Task<StudentMAUI.Models.ConData.Gender> GetGenderByGenderId(int genderid)
        {
            var items = Context.Genders
                              .AsNoTracking()
                              .Where(i => i.GenderID == genderid);

 
            OnGetGenderByGenderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGenderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGenderCreated(StudentMAUI.Models.ConData.Gender item);
        partial void OnAfterGenderCreated(StudentMAUI.Models.ConData.Gender item);

        public async Task<StudentMAUI.Models.ConData.Gender> CreateGender(StudentMAUI.Models.ConData.Gender gender)
        {
            OnGenderCreated(gender);

            var existingItem = Context.Genders
                              .Where(i => i.GenderID == gender.GenderID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Genders.Add(gender);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(gender).State = EntityState.Detached;
                throw;
            }

            OnAfterGenderCreated(gender);

            return gender;
        }

        public async Task<StudentMAUI.Models.ConData.Gender> CancelGenderChanges(StudentMAUI.Models.ConData.Gender item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGenderUpdated(StudentMAUI.Models.ConData.Gender item);
        partial void OnAfterGenderUpdated(StudentMAUI.Models.ConData.Gender item);

        public async Task<StudentMAUI.Models.ConData.Gender> UpdateGender(int genderid, StudentMAUI.Models.ConData.Gender gender)
        {
            OnGenderUpdated(gender);

            var itemToUpdate = Context.Genders
                              .Where(i => i.GenderID == gender.GenderID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(gender);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGenderUpdated(gender);

            return gender;
        }

        partial void OnGenderDeleted(StudentMAUI.Models.ConData.Gender item);
        partial void OnAfterGenderDeleted(StudentMAUI.Models.ConData.Gender item);

        public async Task<StudentMAUI.Models.ConData.Gender> DeleteGender(int genderid)
        {
            var itemToDelete = Context.Genders
                              .Where(i => i.GenderID == genderid)
                              .Include(i => i.Students)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnGenderDeleted(itemToDelete);


            Context.Genders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGenderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportStudentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportStudentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnStudentsRead(ref IQueryable<StudentMAUI.Models.ConData.Student> items);

        public async Task<IQueryable<StudentMAUI.Models.ConData.Student>> GetStudents(Query query = null)
        {
            var items = Context.Students.AsQueryable();

            items = items.Include(i => i.Gender);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnStudentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnStudentGet(StudentMAUI.Models.ConData.Student item);
        partial void OnGetStudentByStudentId(ref IQueryable<StudentMAUI.Models.ConData.Student> items);


        public async Task<StudentMAUI.Models.ConData.Student> GetStudentByStudentId(int studentid)
        {
            var items = Context.Students
                              .AsNoTracking()
                              .Where(i => i.StudentID == studentid);

            items = items.Include(i => i.Gender);
 
            OnGetStudentByStudentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnStudentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnStudentCreated(StudentMAUI.Models.ConData.Student item);
        partial void OnAfterStudentCreated(StudentMAUI.Models.ConData.Student item);

        public async Task<StudentMAUI.Models.ConData.Student> CreateStudent(StudentMAUI.Models.ConData.Student student)
        {
            OnStudentCreated(student);

            var existingItem = Context.Students
                              .Where(i => i.StudentID == student.StudentID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Students.Add(student);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(student).State = EntityState.Detached;
                throw;
            }

            OnAfterStudentCreated(student);

            return student;
        }

        public async Task<StudentMAUI.Models.ConData.Student> CancelStudentChanges(StudentMAUI.Models.ConData.Student item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnStudentUpdated(StudentMAUI.Models.ConData.Student item);
        partial void OnAfterStudentUpdated(StudentMAUI.Models.ConData.Student item);

        public async Task<StudentMAUI.Models.ConData.Student> UpdateStudent(int studentid, StudentMAUI.Models.ConData.Student student)
        {
            OnStudentUpdated(student);

            var itemToUpdate = Context.Students
                              .Where(i => i.StudentID == student.StudentID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(student);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterStudentUpdated(student);

            return student;
        }

        partial void OnStudentDeleted(StudentMAUI.Models.ConData.Student item);
        partial void OnAfterStudentDeleted(StudentMAUI.Models.ConData.Student item);

        public async Task<StudentMAUI.Models.ConData.Student> DeleteStudent(int studentid)
        {
            var itemToDelete = Context.Students
                              .Where(i => i.StudentID == studentid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnStudentDeleted(itemToDelete);


            Context.Students.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterStudentDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}