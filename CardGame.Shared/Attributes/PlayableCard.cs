using System;

namespace CardGame.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayableCard : Attribute
    {
        public Type InstanceType { get; set; }

        public PlayableCard(Type type)
        {
            InstanceType = type;
        }
    }
}
