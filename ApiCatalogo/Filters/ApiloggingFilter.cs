using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiCatalogo.Filters
{
    public class ApiloggingFilter : IActionFilter
    {
        private readonly ILogger<ApiloggingFilter> _logger;

        public ApiloggingFilter(ILogger<ApiloggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuted");
            _logger.LogInformation("############################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"Model State: {context.ModelState.IsValid}");
            _logger.LogInformation("############################################################");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuting");
            _logger.LogInformation("############################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"Status Code: {context.HttpContext.Response.StatusCode}");
            _logger.LogInformation("############################################################");
        }
    }
}
