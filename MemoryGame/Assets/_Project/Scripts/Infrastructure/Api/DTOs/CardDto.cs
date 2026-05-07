using System;

namespace MemoryGame.Client.Infrastructure.Api.DTOs
{
    [Serializable]
    public sealed class CardDto
    {
        public string instanceId;
        public string matchId;
        public string name;
        public string imageUrl;
    }
}
