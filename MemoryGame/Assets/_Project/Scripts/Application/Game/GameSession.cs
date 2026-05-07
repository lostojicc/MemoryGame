using MemoryGame.Client.Domain.Game;

namespace MemoryGame.Client.Application.Game
{
    public sealed class GameSession
    {
        public GameState CurrentGame { get; private set; }

        public bool HasActiveGame
        {
            get { return CurrentGame != null; }
        }

        public void SetCurrentGame(GameState gameState)
        {
            CurrentGame = gameState;
        }

        public void ClearCurrentGame()
        {
            CurrentGame = null;
        }
    }
}
