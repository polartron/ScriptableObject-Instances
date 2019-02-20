using System;
using Fasteraune.Variables;

[Serializable]
public class PlayerDataReference : BaseReference<Player.PlayerData, PlayerDataVariable>
{
    public PlayerDataReference(Player.PlayerData Value) : base(Value)
    {
    }

    public PlayerDataReference()
    {
    }
}