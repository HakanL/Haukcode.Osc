namespace Rug.Osc.Namespaces.Tests
{
    public static class NamespaceTestHelper
    {
        public const string NamePath0 = "/";

        public const string NamePath1 = "/the-doop";

        public const string ParentPath0 = "/";

        public const string ParentPath1 = "/moop";

        public const string ParentPath2 = "/.test-parent";

        public const string ParentPath3 = "/.test-namespace";

        public static readonly string[] InvalidAddresses = new string[]
        {
            null,
            "",
            "/*themoop",
        };

        public static readonly string[] ValidNamespaceAddresses = new string[]
        {
            "/",
            "/moop",
            "/the/doop",
        };

        public static readonly string[] ValidObjectAddresses = new string[]
        {
            "/moop",
            "/the/doop",
        };

        public static Namespace CreateParent(string address)
        {
            return new Namespace() { Name = address };
        }

        public static NamespaceTestNamespace CreateTestNamespaceParent(string address)
        {
            return new NamespaceTestNamespace(address);
        }

        //public static string GetAddress(string namespaceAddress, string objectAddress)
        //{
        //    string address;

        //    if (namespaceAddress == "/")
        //    {
        //        address = objectAddress;
        //    }
        //    else
        //    {
        //        address = namespaceAddress + objectAddress;
        //    }

        //    return address;
        //}

        public static string GetAddress(params string[] namespaceAddressParts)
        {
            string address = "";

            foreach (string part in namespaceAddressParts)
            {
                if (part == "/")
                {
                    continue;
                }
                else
                {
                    address += part;
                }
            }

            return address;
        }
    }
}