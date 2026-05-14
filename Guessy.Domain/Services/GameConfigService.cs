using Guessy.Domain.Models;
using Guessy.Domain.Services.Interfaces;

namespace Guessy.Domain.Services;

public class GameConfigService : IGameConfigService
{
    public GameConfig Config { get; private set; } = GameConfig.Default;

    public void Update(GameConfig config)
    {
        Config = config;
    }
}