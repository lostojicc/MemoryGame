using System;
using MemoryGame.Client.Infrastructure.Configuration;

namespace MemoryGame.Client.Infrastructure.Api
{
    public sealed class ScriptableObjectApiBaseUrlProvider : IApiBaseUrlProvider
    {
        private readonly ApiSettings _apiSettings;

        public ScriptableObjectApiBaseUrlProvider(ApiSettings apiSettings)
        {
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        public string GetBaseUrl()
        {
            return _apiSettings.GetBaseUrl();
        }
    }
}
