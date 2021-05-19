using System;

namespace CardGame.Shared.Enums
{
    /// <summary>
    /// Features that a card can have. Cards can have a combination of features, or none.
    /// </summary>
    [Flags]
    public enum CardFeature
    {
        None = 0,
        Taunt = 1,
        Rush = 2
    }
}
