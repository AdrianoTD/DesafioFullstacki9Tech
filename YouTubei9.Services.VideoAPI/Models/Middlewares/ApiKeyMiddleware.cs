namespace YouTubei9.Services.VideoAPI.Models.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "Authorization";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Method == "OPTIONS")
            {
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

            var isValid = await ValidateApiKeyAsync(apiKey);

            if (!isValid)
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsync("CHAVE DE API INVÁLIDA!");
                return;
            }

            httpContext.Items["ApiKey"] = apiKey;

            await _next(httpContext);
        }

        private async Task<bool> ValidateApiKeyAsync(string apiKey)
        {
            var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=dotnet8&type=video&relevanceLanguage=pt&publishedAfter=2025-01-01T00:00:00Z&maxResults=15&key={apiKey}";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
