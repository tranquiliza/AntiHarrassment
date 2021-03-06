﻿using AntiHarassment.Core;
using AntiHarassment.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Filters
{
    public class ApplicationContextFilter : IAsyncActionFilter
    {
        private readonly IUserRepository userRepository;

        public ApplicationContextFilter(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Might need synchronisation context")]
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is ContextController controller && Guid.TryParse(context.HttpContext?.User?.Identity?.Name, out var userId))
            {
                controller.CurrentUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.PathBase}";

                var user = await userRepository.GetById(userId).ConfigureAwait(false);
                if (user.IsLocked)
                {
                    context.Result = new BadRequestObjectResult("User has been locked");
                    return;
                }

                controller.ApplicationContext = ApplicationContext.Create(user);
            }

            await next.Invoke();
        }
    }
}
