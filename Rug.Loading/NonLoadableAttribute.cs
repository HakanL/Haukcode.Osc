using System;

namespace Rug.Loading
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NonLoadableAttribute : Attribute { }
}