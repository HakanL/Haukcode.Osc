using System;

namespace Rug.Loading
{
    public class AliasAttribute : Attribute
    {
        public readonly string[] Alias;

        public AliasAttribute(params string[] aliases)
        {
            Alias = aliases;
        }
    }
}