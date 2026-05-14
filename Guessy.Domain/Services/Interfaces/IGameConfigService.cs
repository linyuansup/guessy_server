using Guessy.Domain.Models;

namespace Guessy.Domain.Services.Interfaces;

public interface IGameConfigService
{
    GameConfig Config { get; }
    void Update(GameConfig config);
}