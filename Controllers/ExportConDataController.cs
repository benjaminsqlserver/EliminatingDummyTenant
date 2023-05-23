using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using AzureADTenantPart2.Data;

namespace AzureADTenantPart2.Controllers
{
    public partial class ExportConDataController : ExportController
    {
        private readonly ConDataContext context;
        private readonly ConDataService service;

        public ExportConDataController(ConDataContext context, ConDataService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ConData/allowedtenants/csv")]
        [HttpGet("/export/ConData/allowedtenants/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAllowedTenantsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAllowedTenants(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/allowedtenants/excel")]
        [HttpGet("/export/ConData/allowedtenants/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAllowedTenantsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAllowedTenants(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/offices/csv")]
        [HttpGet("/export/ConData/offices/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOfficesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOffices(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/offices/excel")]
        [HttpGet("/export/ConData/offices/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOfficesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOffices(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/solutionusers/csv")]
        [HttpGet("/export/ConData/solutionusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSolutionUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSolutionUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/solutionusers/excel")]
        [HttpGet("/export/ConData/solutionusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSolutionUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSolutionUsers(), Request.Query), fileName);
        }
    }
}
