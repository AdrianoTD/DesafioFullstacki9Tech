using Microsoft.Extensions.DependencyInjection;
using YouTubei9.Services.VideoAPI.Services;

namespace YouTubei9.Services.VideoAPI.Models.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const string ApiKeyHeaderName = "Authorization";

        public ApiKeyMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Method == "OPTIONS")
            {
                await _next(httpContext);
                return;
            }

            if (httpContext.Request.Headers.TryGetValue("IsAuthorized", out var isAuthorizedHeader) &&
            bool.TryParse(isAuthorizedHeader.ToString(), out var isAuthorized) && isAuthorized)
            {
                // Se "IsAuthorized" for true, permite a requisição
                await _next(httpContext);
                return;
            }

            if (!httpContext.Request.Headers.ContainsKey(ApiKeyHeaderName))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("INSIRA UMA CHAVE DE API PARA YOUTUBE!");
                return;
            }

            var apiKey = httpContext.Request.Headers[ApiKeyHeaderName].ToString();

            using var scope = _serviceScopeFactory.CreateScope();
            var validationService = scope.ServiceProvider.GetRequiredService<ApiKeyValidationService>();
            var isValid = await validationService.ValidateApiKeyAsync(apiKey);

            if (!isValid)
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsync("CHAVE DE API INVÁLIDA!");
                return;
            }

            httpContext.Items["ApiKey"] = apiKey;

            await _next(httpContext);
        }
    }
}
