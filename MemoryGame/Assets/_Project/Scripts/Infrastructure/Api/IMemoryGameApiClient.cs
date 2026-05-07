using System.Threading;
using System.Threading.Tasks;
using MemoryGame.Client.Infrastructure.Api.DTOs;

namespace MemoryGame.Client.Infrastructure.Api
{
    public interface IMemoryGameApiClient
    {
        Task<GameDto> CreateGame(int pairs = 0, CancellationToken cancellationToken = default);
    }
}
