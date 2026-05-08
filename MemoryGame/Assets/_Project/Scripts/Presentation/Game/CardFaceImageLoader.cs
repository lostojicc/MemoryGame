using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class CardFaceImageLoader
    {
        private readonly Dictionary<string, Sprite> _spritesByUrl = new();

        public async Task<Sprite> LoadSprite(string imageUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            if (_spritesByUrl.TryGetValue(imageUrl, out Sprite cachedSprite))
                return cachedSprite;

            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException($"Failed to load card image. URL: {imageUrl}. Error: {request.error}");

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

            _spritesByUrl[imageUrl] = sprite;
            return sprite;
        }
    }
}
