using System;
using UnityEngine;

namespace MemoryGame.Client.Infrastructure.Configuration
{
    [CreateAssetMenu(
        fileName = "ApiSettings",
        menuName = "Memory Game/Configuration/API Settings")]
    public sealed class ApiSettings : ScriptableObject
    {
        [Header("Environment")]
        [SerializeField] private ApiEnvironment environment = ApiEnvironment.Render;

        [Header("Base URLs")]
        [SerializeField] private string localBaseUrl = "http://localhost:3000";
        [SerializeField] private string renderBaseUrl = "https://your-render-url.onrender.com";

        [Header("Override")]
        [SerializeField] private bool useOverride;
        [SerializeField] private string overrideBaseUrl = string.Empty;

        public string GetBaseUrl()
        {
            string selectedBaseUrl = useOverride
                ? overrideBaseUrl
                : environment switch
                {
                    ApiEnvironment.Local => localBaseUrl,
                    ApiEnvironment.Render => renderBaseUrl,
                    _ => throw new ArgumentOutOfRangeException()
                };

            if (string.IsNullOrWhiteSpace(selectedBaseUrl))
                throw new InvalidOperationException("API base URL must be configured.");

            return selectedBaseUrl.Trim().TrimEnd('/');
        }
    }
}
