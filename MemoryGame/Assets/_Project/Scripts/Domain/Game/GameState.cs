using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame.Client.Domain.Game
{
    public sealed class GameState
    {
        private readonly List<CardState> _cards;
        private CardState _firstSelectedCard;

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

        public CardSelectionResult SelectCard(CardState card)
        {
            if (card == null || card.IsMatched || card.IsRevealed)
                return CardSelectionResult.Ignored();

            card.Reveal();

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
                return CardSelectionResult.FirstCardSelected(card);
            }

            CardState firstCard = _firstSelectedCard;
            CardState secondCard = card;
            _firstSelectedCard = null;

            Attempts++;
            bool isMatch = firstCard.MatchId == secondCard.MatchId;

            if (isMatch)
            {
                firstCard.MarkAsMatched();
                secondCard.MarkAsMatched();
                Score++;
                Combo++;

                if (IsComplete)
                    return CardSelectionResult.GameCompleted(firstCard, secondCard);

                return CardSelectionResult.MatchFound(firstCard, secondCard);
            }

            Combo = 0;
            return CardSelectionResult.MismatchFound(firstCard, secondCard);
        }

        public void HideUnmatchedCards(CardState firstCard, CardState secondCard)
        {
            firstCard?.Hide();
            secondCard?.Hide();
        }
    }
}
