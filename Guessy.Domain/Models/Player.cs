using Guessy.Domain.ValueObjects.Game;
using Guessy.Domain.ValueObjects.Ids;

namespace Guessy.Domain.Models;

public class Player(BraceletId braceletId, DeviceId deviceId)
{
    public BraceletId BraceletId { get; private set; } = braceletId;
    public DeviceId DeviceId { get; private set; } = deviceId;
    public Score Score { get; private set; } = new(0);

    public void AddScore(int value)
    {
        if (value == 0)
        {
            return;
        }

        Score = new Score(Score.Value + value);
    }
}