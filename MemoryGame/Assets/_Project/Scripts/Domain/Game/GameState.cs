using System.Collections.Generic;

namespace MemoryGame.Client.Domain.Game
{
    public sealed class GameState
    {
        private readonly List<CardState> _cards;

        public string GameId { get; }
        public int Pairs { get; }
        public int Attempts { get; private set; }
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public IReadOnlyList<CardState> Cards => _cards;

        public bool IsComplete
        {
            get { return Score >= Pairs; }
        }

        public GameState(string gameId, int pairs, List<CardState> cards)
        {
            GameId = gameId;
            Pairs = pairs;
            _cards = cards;
        }

        public bool ResolveAttempt(CardState firstCard, CardState secondCard)
        {
            Attempts++;

            bool isMatch = firstCard.MatchId == secondCard.MatchId;

            if (isMatch)
            {
                firstCard.MarkAsMatched();
                secondCard.MarkAsMatched();
                Score++;
                Combo++;
                return true;
            }

            Combo = 0;
            return false;
        }
    }
}
