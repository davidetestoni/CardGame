using System;

namespace CardGame.Shared.Enums
{
    [Flags]
    public enum CardFeature
    {
        None = 0,
        Taunt = 1,
        Rush = 2
    }
}
