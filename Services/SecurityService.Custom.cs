using Microsoft.AspNetCore.Components;

namespace AzureADTenantPart2
{
    public partial class SecurityService
    {

        public async Task<ConDataService> CreateDataServiceForUser(NavigationManager navigationManager)
        {
            //to prevent data leak another tenant seeing other tenant's data

            var service = new ConDataService(navigationManager, _multitenancy);

            //get preferred username claim which is in email format

            var usernameClaim = Principal.Claims.Where(p => p.Type == "preferred_username").FirstOrDefault();
            bool userIsValid = false;
            if (usernameClaim != null)
            {
                userIsValid = await service.UserIsValid(usernameClaim.Value);
            }



            if (userIsValid)
            {
                return service;
            }
            else//leaving this the way it is in case developer forgets to carryout the check whether user is accessing the right url on the page
            {
                return new ConDataService(_multitenancy.Tenants[2].ConnectionString);
            }



        }

        public async Task<bool> UserIsAccessingAssignedURL(NavigationManager navigationManager)
        {

            //create a new data access service using navigation manager and multitenancy session from app settings

            var service = new ConDataService(navigationManager, _multitenancy);

            //get preferred username claim which is in email format

            var usernameClaim = Principal.Claims.Where(p => p.Type == "preferred_username").FirstOrDefault();

            //declare boolean variable userIsValid and assign it initial value of false
            bool userIsValid = false;
            //if username claim is not null check whether user is valid
            if (usernameClaim != null)
            {
                userIsValid = await service.UserIsValid(usernameClaim.Value);
            }


            //if user is valid return true
            if (userIsValid)
            {
                return true;
            }
            //otherwise return false
            else
            {
                return false;
            }

        }

    }
}
