using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiHarassment.Core;
using AntiHarassment.Core.Security;
using AntiHarassment.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SystemController : ContextController
    {
        private readonly ISystemReportService systemReportService;

        public SystemController(ISystemReportService systemReportService)
        {
            this.systemReportService = systemReportService;
        }

        [HttpGet("report")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetSystemReport()
        {
            var result = await systemReportService.GetSystemReport(ApplicationContext).ConfigureAwait(false);
            if (result.State == ResultState.AccessDenied)
                return Unauthorized();

            if (result.State == ResultState.NoContent)
                return NoContent();

            return Ok(result.Data.Map());
        }
    }
}
