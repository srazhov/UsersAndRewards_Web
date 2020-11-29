using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace UsersAndRewards.Filters
{
    public class ProtocolUser : Attribute, IActionFilter
    {
        readonly ILogger logger;

        public ProtocolUser(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("UserLoger");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation($"User logged out from {context.ActionDescriptor.DisplayName} in {DateTime.Now}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation($"User logged in from {context.ActionDescriptor.DisplayName} in {DateTime.Now}");
        }
    }
}
