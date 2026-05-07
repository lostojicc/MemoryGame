using System.Threading;
using System.Threading.Tasks;
using MemoryGame.Client.Domain.Game;

namespace MemoryGame.Client.Application.Game
{
    public interface IStartGameUseCase
    {
        const int DefaultPairs = 10;

        Task<GameState> Execute(int pairs = DefaultPairs, CancellationToken cancellationToken = default);
    }
}
