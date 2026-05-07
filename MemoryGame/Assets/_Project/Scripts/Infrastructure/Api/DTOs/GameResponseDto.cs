using System;

namespace MemoryGame.Client.Infrastructure.Api.DTOs
{
    [Serializable]
    public sealed class GameResponseDto
    {
        public bool success;
        public GameDto game;
        public string error;
    }
}
