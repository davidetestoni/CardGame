using System;

namespace CardGame.Shared.Attributes
{
    /// <summary>
    /// As long as this card is on the field, other allied creatures and the player
    /// cannot be attacked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Taunt : Attribute
    {
        
    }
}
