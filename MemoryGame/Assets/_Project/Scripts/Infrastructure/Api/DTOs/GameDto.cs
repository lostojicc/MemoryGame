using System;

namespace MemoryGame.Client.Infrastructure.Api.DTOs
{
    [Serializable]
    public sealed class GameDto
    {
        public string gameId;
        public int pairs;
        public int cardCount;
        public CardDto[] cards;
    }
}
