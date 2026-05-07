namespace MemoryGame.Client.Domain.Game
{
    public sealed class CardState
    {
        public string InstanceId { get; }
        public string MatchId { get; }
        public string Name { get; }
        public string ImageUrl { get; }
        public bool IsRevealed { get; private set; }
        public bool IsMatched { get; private set; }

        public CardState(string instanceId, string matchId, string name, string imageUrl)
        {
            InstanceId = instanceId;
            MatchId = matchId;
            Name = name;
            ImageUrl = imageUrl;
        }

        public void Reveal()
        {
            if (!IsMatched)
                IsRevealed = true;
        }

        public void Hide()
        {
            if (!IsMatched)
                IsRevealed = false;
        }

        public void MarkAsMatched()
        {
            IsMatched = true;
            IsRevealed = true;
        }
    }
}
