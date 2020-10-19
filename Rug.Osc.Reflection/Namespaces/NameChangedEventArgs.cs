using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Namespaces
{
    public class NameChangedEventArgs
    {
        public readonly string OldAddress;

        public readonly string NewAddress;

        public NameChangedEventArgs(string oldAddress, string newAddress)
        {
            OldAddress = oldAddress;
            NewAddress = newAddress; 
        }
    }
}
