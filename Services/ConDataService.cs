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

using AzureADTenantPart2.Data;

namespace AzureADTenantPart2
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
            get
            {
                return this.context;
            }
            set
            {
                this.context = new ConDataContext();

            }
        }

        private  ConDataContext context;
        private readonly NavigationManager navigationManager;
        private string connectionString;
        private readonly Multitenancy _multitenancy;

        //public ConDataService(ConDataContext context, NavigationManager navigationManager)
        //{
        //    this.context = context;
        //    this.navigationManager = navigationManager;
        //}
        public ConDataService(NavigationManager navigationManager, Multitenancy multitenancy)
        {
            if (navigationManager != null)
            {
                this.navigationManager = navigationManager;
                var appURL = "";
                try
                {
                    appURL = this.navigationManager.BaseUri;
                    _multitenancy = multitenancy;
                    //this.context.Database.
                    if (_multitenancy != null)
                    {
                        var tenant = multitenancy.Tenants
                               .Where(t => t.Hostnames.Contains(appURL)).FirstOrDefault();
                        if (tenant != null)
                        {
                            try
                            {
                                if (this.context == null)
                                {
                                    this.context = new ConDataContext(tenant.ConnectionString);
                                }

                            }
                            catch (Exception ex)
                            {
                                throw;
                            }


                        }
                    }
                }
                catch
                {
                    this.context = new ConDataContext(multitenancy.Tenants[0].ConnectionString);
                }





            }


        }

        public ConDataService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _multitenancy.Tenants[0].ConnectionString;
            }
            this.context = new ConDataContext(connectionString);
        }


       
        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);


        public async Task ExportAllowedTenantsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/allowedtenants/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/allowedtenants/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAllowedTenantsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/allowedtenants/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/allowedtenants/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAllowedTenantsRead(ref IQueryable<AzureADTenantPart2.Models.ConData.AllowedTenant> items);

        public async Task<IQueryable<AzureADTenantPart2.Models.ConData.AllowedTenant>> GetAllowedTenants(Query query = null)
        {
            var items = Context.AllowedTenants.AsQueryable();


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

            OnAllowedTenantsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAllowedTenantGet(AzureADTenantPart2.Models.ConData.AllowedTenant item);
        partial void OnGetAllowedTenantByTenantId(ref IQueryable<AzureADTenantPart2.Models.ConData.AllowedTenant> items);


        public async Task<AzureADTenantPart2.Models.ConData.AllowedTenant> GetAllowedTenantByTenantId(long tenantid)
        {
            var items = Context.AllowedTenants
                              .AsNoTracking()
                              .Where(i => i.TenantID == tenantid);

 
            OnGetAllowedTenantByTenantId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAllowedTenantGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAllowedTenantCreated(AzureADTenantPart2.Models.ConData.AllowedTenant item);
        partial void OnAfterAllowedTenantCreated(AzureADTenantPart2.Models.ConData.AllowedTenant item);

        public async Task<AzureADTenantPart2.Models.ConData.AllowedTenant> CreateAllowedTenant(AzureADTenantPart2.Models.ConData.AllowedTenant allowedtenant)
        {
            OnAllowedTenantCreated(allowedtenant);

            var existingItem = Context.AllowedTenants
                              .Where(i => i.TenantID == allowedtenant.TenantID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AllowedTenants.Add(allowedtenant);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(allowedtenant).State = EntityState.Detached;
                throw;
            }

            OnAfterAllowedTenantCreated(allowedtenant);

            return allowedtenant;
        }

        public async Task<AzureADTenantPart2.Models.ConData.AllowedTenant> CancelAllowedTenantChanges(AzureADTenantPart2.Models.ConData.AllowedTenant item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAllowedTenantUpdated(AzureADTenantPart2.Models.ConData.AllowedTenant item);
        partial void OnAfterAllowedTenantUpdated(AzureADTenantPart2.Models.ConData.AllowedTenant item);

        public async Task<AzureADTenantPart2.Models.ConData.AllowedTenant> UpdateAllowedTenant(long tenantid, AzureADTenantPart2.Models.ConData.AllowedTenant allowedtenant)
        {
            OnAllowedTenantUpdated(allowedtenant);

            var itemToUpdate = Context.AllowedTenants
                              .Where(i => i.TenantID == allowedtenant.TenantID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(allowedtenant);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAllowedTenantUpdated(allowedtenant);

            return allowedtenant;
        }

        partial void OnAllowedTenantDeleted(AzureADTenantPart2.Models.ConData.AllowedTenant item);
        partial void OnAfterAllowedTenantDeleted(AzureADTenantPart2.Models.ConData.AllowedTenant item);

        public async Task<AzureADTenantPart2.Models.ConData.AllowedTenant> DeleteAllowedTenant(long tenantid)
        {
            var itemToDelete = Context.AllowedTenants
                              .Where(i => i.TenantID == tenantid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAllowedTenantDeleted(itemToDelete);


            Context.AllowedTenants.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAllowedTenantDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOfficesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/offices/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/offices/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOfficesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/offices/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/offices/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOfficesRead(ref IQueryable<AzureADTenantPart2.Models.ConData.Office> items);

        public async Task<IQueryable<AzureADTenantPart2.Models.ConData.Office>> GetOffices(Query query = null)
        {
            var items = Context.Offices.AsQueryable();


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

            OnOfficesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOfficeGet(AzureADTenantPart2.Models.ConData.Office item);
        partial void OnGetOfficeByOfficeId(ref IQueryable<AzureADTenantPart2.Models.ConData.Office> items);


        public async Task<AzureADTenantPart2.Models.ConData.Office> GetOfficeByOfficeId(int officeid)
        {
            var items = Context.Offices
                              .AsNoTracking()
                              .Where(i => i.OfficeID == officeid);

 
            OnGetOfficeByOfficeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOfficeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOfficeCreated(AzureADTenantPart2.Models.ConData.Office item);
        partial void OnAfterOfficeCreated(AzureADTenantPart2.Models.ConData.Office item);

        public async Task<AzureADTenantPart2.Models.ConData.Office> CreateOffice(AzureADTenantPart2.Models.ConData.Office office)
        {
            OnOfficeCreated(office);

            var existingItem = Context.Offices
                              .Where(i => i.OfficeID == office.OfficeID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Offices.Add(office);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(office).State = EntityState.Detached;
                throw;
            }

            OnAfterOfficeCreated(office);

            return office;
        }

        public async Task<AzureADTenantPart2.Models.ConData.Office> CancelOfficeChanges(AzureADTenantPart2.Models.ConData.Office item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOfficeUpdated(AzureADTenantPart2.Models.ConData.Office item);
        partial void OnAfterOfficeUpdated(AzureADTenantPart2.Models.ConData.Office item);

        public async Task<AzureADTenantPart2.Models.ConData.Office> UpdateOffice(int officeid, AzureADTenantPart2.Models.ConData.Office office)
        {
            OnOfficeUpdated(office);

            var itemToUpdate = Context.Offices
                              .Where(i => i.OfficeID == office.OfficeID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(office);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOfficeUpdated(office);

            return office;
        }

        partial void OnOfficeDeleted(AzureADTenantPart2.Models.ConData.Office item);
        partial void OnAfterOfficeDeleted(AzureADTenantPart2.Models.ConData.Office item);

        public async Task<AzureADTenantPart2.Models.ConData.Office> DeleteOffice(int officeid)
        {
            var itemToDelete = Context.Offices
                              .Where(i => i.OfficeID == officeid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOfficeDeleted(itemToDelete);


            Context.Offices.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOfficeDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSolutionUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/solutionusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/solutionusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSolutionUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/solutionusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/solutionusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSolutionUsersRead(ref IQueryable<AzureADTenantPart2.Models.ConData.SolutionUser> items);

        public async Task<IQueryable<AzureADTenantPart2.Models.ConData.SolutionUser>> GetSolutionUsers(Query query = null)
        {
            var items = Context.SolutionUsers.AsQueryable();


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

            OnSolutionUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSolutionUserGet(AzureADTenantPart2.Models.ConData.SolutionUser item);
        partial void OnGetSolutionUserByUserId(ref IQueryable<AzureADTenantPart2.Models.ConData.SolutionUser> items);


        public async Task<AzureADTenantPart2.Models.ConData.SolutionUser> GetSolutionUserByUserId(long userid)
        {
            var items = Context.SolutionUsers
                              .AsNoTracking()
                              .Where(i => i.UserID == userid);

 
            OnGetSolutionUserByUserId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSolutionUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSolutionUserCreated(AzureADTenantPart2.Models.ConData.SolutionUser item);
        partial void OnAfterSolutionUserCreated(AzureADTenantPart2.Models.ConData.SolutionUser item);

        public async Task<AzureADTenantPart2.Models.ConData.SolutionUser> CreateSolutionUser(AzureADTenantPart2.Models.ConData.SolutionUser solutionuser)
        {
            OnSolutionUserCreated(solutionuser);

            var existingItem = Context.SolutionUsers
                              .Where(i => i.UserID == solutionuser.UserID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.SolutionUsers.Add(solutionuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(solutionuser).State = EntityState.Detached;
                throw;
            }

            OnAfterSolutionUserCreated(solutionuser);

            return solutionuser;
        }

        public async Task<AzureADTenantPart2.Models.ConData.SolutionUser> CancelSolutionUserChanges(AzureADTenantPart2.Models.ConData.SolutionUser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSolutionUserUpdated(AzureADTenantPart2.Models.ConData.SolutionUser item);
        partial void OnAfterSolutionUserUpdated(AzureADTenantPart2.Models.ConData.SolutionUser item);

        public async Task<AzureADTenantPart2.Models.ConData.SolutionUser> UpdateSolutionUser(long userid, AzureADTenantPart2.Models.ConData.SolutionUser solutionuser)
        {
            OnSolutionUserUpdated(solutionuser);

            var itemToUpdate = Context.SolutionUsers
                              .Where(i => i.UserID == solutionuser.UserID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(solutionuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSolutionUserUpdated(solutionuser);

            return solutionuser;
        }

        partial void OnSolutionUserDeleted(AzureADTenantPart2.Models.ConData.SolutionUser item);
        partial void OnAfterSolutionUserDeleted(AzureADTenantPart2.Models.ConData.SolutionUser item);

        public async Task<AzureADTenantPart2.Models.ConData.SolutionUser> DeleteSolutionUser(long userid)
        {
            var itemToDelete = Context.SolutionUsers
                              .Where(i => i.UserID == userid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSolutionUserDeleted(itemToDelete);


            Context.SolutionUsers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSolutionUserDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}