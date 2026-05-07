using MemoryGame.Client.Application.Game;
using MemoryGame.Client.Infrastructure.Api;
using MemoryGame.Client.Infrastructure.Configuration;
using UnityEngine;

namespace MemoryGame.Client.Composition
{
    public sealed class ClientCompositionRoot : MonoBehaviour
    {
        [SerializeField] private ApiSettings apiSettings = null;

        public static ClientCompositionRoot Instance { get; private set; }

        public IApiBaseUrlProvider ApiBaseUrlProvider { get; private set; }
        public IMemoryGameApiClient MemoryGameApiClient { get; private set; }
        public GameSession GameSession { get; private set; }
        public IStartGameUseCase StartGameUseCase { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (apiSettings == null)
                throw new MissingReferenceException("API settings asset is not assigned.");

            Instance = this;
            DontDestroyOnLoad(gameObject);

            ApiBaseUrlProvider = new ScriptableObjectApiBaseUrlProvider(apiSettings);
            MemoryGameApiClient = new MemoryGameApiClient(ApiBaseUrlProvider);
            GameSession = new GameSession();
            StartGameUseCase = new StartGameUseCase(MemoryGameApiClient, GameSession);
        }
    }
}
