using System;

namespace CardGame.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class Rush : Attribute
    {
        public Rush() { }
    }
}
