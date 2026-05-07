using System;
using System.Threading;
using System.Threading.Tasks;
using MemoryGame.Client.Infrastructure.Api.DTOs;
using UnityEngine;
using UnityEngine.Networking;

namespace MemoryGame.Client.Infrastructure.Api
{
    public sealed class MemoryGameApiClient : IMemoryGameApiClient
    {
        private readonly IApiBaseUrlProvider _apiBaseUrlProvider;

        public MemoryGameApiClient(IApiBaseUrlProvider apiBaseUrlProvider)
        {
            _apiBaseUrlProvider = apiBaseUrlProvider ?? throw new ArgumentNullException(nameof(apiBaseUrlProvider));
        }

        public async Task<GameDto> CreateGame(int pairs = 0, CancellationToken cancellationToken = default)
        {
            string requestUrl = BuildRequestUrl(pairs);

            using UnityWebRequest request = UnityWebRequest.Get(requestUrl);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException($"Failed to create memory game. URL: {requestUrl}. Error: {request.error}");

            GameResponseDto response = JsonUtility.FromJson<GameResponseDto>(request.downloadHandler.text);

            if (response == null)
                throw new InvalidOperationException("Game response could not be deserialized.");

            if (!response.success)
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(response.error) ? "Server rejected the game request." : response.error);

            if (response.game == null || response.game.cards == null || response.game.cards.Length == 0)
                throw new InvalidOperationException("Server returned a game without cards.");

            return response.game;
        }

        private string BuildRequestUrl(int pairs)
        {
            string requestUrl = $"{_apiBaseUrlProvider.GetBaseUrl()}/api/games/new";

            if (pairs > 0)
                requestUrl += $"?pairs={pairs}";

            return requestUrl;
        }
    }
}
