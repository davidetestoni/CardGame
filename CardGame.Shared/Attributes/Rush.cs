using System;

namespace CardGame.Shared.Attributes
{
    /// <summary>
    /// This card can attack in the same turn in which it's played.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Rush : Attribute
    {

    }
}
