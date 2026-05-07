using System;
using MemoryGame.Client.Composition;
using UnityEngine;

namespace MemoryGame.Client.Presentation.Shared
{
    public sealed class StartGameProbe : MonoBehaviour
    {
        [SerializeField] private int pairs;

        private async void Start()
        {
            try
            {
                if (ClientCompositionRoot.Instance == null)
                {
                    Debug.LogError("ClientCompositionRoot instance is not available.");
                    return;
                }

                var gameState = await ClientCompositionRoot.Instance.StartGameUseCase.Execute(pairs);

                Debug.Log(
                    $"Game initialized successfully. " +
                    $"Game ID: {gameState.GameId}, " +
                    $"Pairs: {gameState.Pairs}, " +
                    $"Cards: {gameState.Cards.Count}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to initialize game: {exception}");
            }
        }
    }
}
