namespace YouTubei9.Services.VideoAPI.Services
{
    public class ApiKeyValidationService
    {
        public async Task<bool> ValidateApiKeyAsync(string apiKey)
        {
            var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=love&type=video&relevanceLanguage=pt&maxResults=5&key={apiKey}";

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
