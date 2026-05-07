using MemoryGame.Client.Composition;
using MemoryGame.Client.Domain.Game;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class GameSceneController : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private Text scoreText = null;
        [SerializeField] private Text attemptsText = null;
        [SerializeField] private Text timerText = null;
        [SerializeField] private Text cardCountText = null;

        [Header("Board")]
        [SerializeField] private BoardView boardView = null;

        private void Start()
        {
            if (ClientCompositionRoot.Instance == null)
            {
                Debug.LogError("ClientCompositionRoot instance is not available.");
                return;
            }

            GameState gameState = ClientCompositionRoot.Instance.GameSession.CurrentGame;

            if (gameState == null)
            {
                Debug.LogError("No active game found. Start the game from the main menu scene first.");
                return;
            }

            UpdateHud(gameState);
            ShowBoard(gameState);

            Debug.Log(
                $"Game scene loaded. " +
                $"Game ID: {gameState.GameId}, " +
                $"Pairs: {gameState.Pairs}, " +
                $"Cards: {gameState.Cards.Count}");
        }

        private void UpdateHud(GameState gameState)
        {
            SetText(scoreText, $"Score: {gameState.Score}");
            SetText(attemptsText, $"Attempts: {gameState.Attempts}");
            SetText(timerText, "Time: 00:00");
            SetText(cardCountText, $"Cards: {gameState.Cards.Count}");
        }

        private void ShowBoard(GameState gameState)
        {
            if (boardView == null)
            {
                Debug.LogError("GameSceneController is missing a BoardView reference.");
                return;
            }

            boardView.ShowCards(gameState.Cards);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
                target.text = value;
        }
    }
}
