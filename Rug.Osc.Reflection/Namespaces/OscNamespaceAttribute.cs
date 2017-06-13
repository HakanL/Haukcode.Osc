using System;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OscNamespaceAttribute : OscMemberAttribute
    {
        public string Namesapce { get; private set; }

        public OscNamespaceAttribute(string @namespace = "/")
        {
            if (@namespace != "/" && Rug.Osc.OscAddress.IsValidAddressLiteral(@namespace) == false)
            {
                throw new ArgumentException($"Invalid namespace \"{@namespace}\".", nameof(@namespace));
            }

            Namesapce = @namespace;
        }
    }
}