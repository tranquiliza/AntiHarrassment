using AntiHarassment.Core.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Controllers
{
    public class ContextController : ControllerBase
    {
        public IApplicationContext ApplicationContext { get; set; }
    }
}
