using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AI_Content_Assistant.Filters
{
    public class ExecutionTimeFilter : IAsyncActionFilter
    {
        private readonly ILogger<ExecutionTimeFilter> _logger;

        public ExecutionTimeFilter(ILogger<ExecutionTimeFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            // Execute the action
            var resultContext = await next();

            stopwatch.Stop();

            var actionName = context.ActionDescriptor.DisplayName;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("Action {ActionName} executed in {Elapsed} ms", actionName, elapsedMs);
        }
    }
}
