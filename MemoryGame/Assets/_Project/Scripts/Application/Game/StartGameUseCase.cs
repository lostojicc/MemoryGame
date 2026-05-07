using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MemoryGame.Client.Domain.Game;
using MemoryGame.Client.Infrastructure.Api;
using MemoryGame.Client.Infrastructure.Api.DTOs;

namespace MemoryGame.Client.Application.Game
{
    public sealed class StartGameUseCase : IStartGameUseCase
    {
        private readonly IMemoryGameApiClient _memoryGameApiClient;
        private readonly GameSession _gameSession;

        public StartGameUseCase(IMemoryGameApiClient memoryGameApiClient, GameSession gameSession)
        {
            _memoryGameApiClient = memoryGameApiClient ?? throw new ArgumentNullException(nameof(memoryGameApiClient));
            _gameSession = gameSession ?? throw new ArgumentNullException(nameof(gameSession));
        }

        public async Task<GameState> Execute(int pairs = IStartGameUseCase.DefaultPairs, CancellationToken cancellationToken = default)
        {
            GameDto game = await _memoryGameApiClient.CreateGame(pairs, cancellationToken);
            GameState gameState = CreateInitialGameState(game);

            _gameSession.SetCurrentGame(gameState);

            return gameState;
        }

        private static GameState CreateInitialGameState(GameDto game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            List<CardState> cards = new();

            foreach (CardDto card in game.cards)
                cards.Add(new CardState(card.instanceId, card.matchId, card.name, card.imageUrl));

            return new GameState(game.gameId, game.pairs, cards);
        }
    }
}
