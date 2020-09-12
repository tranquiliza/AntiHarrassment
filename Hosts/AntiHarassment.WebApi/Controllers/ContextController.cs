using AntiHarassment.Core.Security;
using Microsoft.AspNetCore.Mvc;

namespace AntiHarassment.WebApi.Controllers
{
    public class ContextController : ControllerBase
    {
        public string CurrentUrl { get; set; }
        public IApplicationContext ApplicationContext { get; set; }
    }
}
