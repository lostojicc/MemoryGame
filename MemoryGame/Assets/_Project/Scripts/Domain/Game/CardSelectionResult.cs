namespace MemoryGame.Client.Domain.Game
{
    public sealed class CardSelectionResult
    {
        public CardSelectionResultType Type { get; }
        public CardState FirstCard { get; }
        public CardState SecondCard { get; }

        private CardSelectionResult(
            CardSelectionResultType type,
            CardState firstCard = null,
            CardState secondCard = null)
        {
            Type = type;
            FirstCard = firstCard;
            SecondCard = secondCard;
        }

        public static CardSelectionResult Ignored()
        {
            return new CardSelectionResult(CardSelectionResultType.Ignored);
        }

        public static CardSelectionResult FirstCardSelected(CardState card)
        {
            return new CardSelectionResult(CardSelectionResultType.FirstCardSelected, card);
        }

        public static CardSelectionResult MatchFound(CardState firstCard, CardState secondCard)
        {
            return new CardSelectionResult(CardSelectionResultType.MatchFound, firstCard, secondCard);
        }

        public static CardSelectionResult MismatchFound(CardState firstCard, CardState secondCard)
        {
            return new CardSelectionResult(CardSelectionResultType.MismatchFound, firstCard, secondCard);
        }

        public static CardSelectionResult GameCompleted(CardState firstCard, CardState secondCard)
        {
            return new CardSelectionResult(CardSelectionResultType.GameCompleted, firstCard, secondCard);
        }
    }
}
