using System;

namespace CardGame.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class Rush : Attribute
    {
        public Rush() { }
    }
}
