using AzureADTenantPart2.Models.ConData;
using Microsoft.EntityFrameworkCore;

namespace AzureADTenantPart2
{
    public partial class ConDataService
    {
        public async Task<SolutionUser> GetSolutionUserByEmail(string name)
        {
            SolutionUser itemToReturn = null;
            if (Context != null)
            {
                var items = Context.SolutionUsers
                            .AsNoTracking()
                            .Where(i => i.EmailAddress == name);


                itemToReturn = items.FirstOrDefault();

                OnSolutionUserGet(itemToReturn);
            }


            return await Task.FromResult(itemToReturn);
        }


        public async Task<bool> UserIsValid(string name)
        {
            var validUser = false;
            if (Context != null)
            {
                var items = Context.SolutionUsers
                             .AsNoTracking()
                             .Where(i => i.EmailAddress == name);


                var itemToReturn = items.FirstOrDefault();

                if (itemToReturn != null)
                {
                    var indexOfAt = name.IndexOf("@");
                    var userDomainName = name.Substring(indexOfAt + 1);
                    //get all allowed domain names 
                    var allowedTenant = await Context.AllowedTenants.FirstOrDefaultAsync(p => p.DomainName.ToLower() == userDomainName.ToLower());
                    if (allowedTenant != null)
                    {
                        validUser = true;
                    }
                }
            }


            return validUser;
        }
    }
}
