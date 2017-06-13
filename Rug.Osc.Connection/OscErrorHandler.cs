using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Connection
{
    public class OscErrorHandler
    {
        private IOscAddressManager oscAddressManager; 

        public OscErrorHandler(IOscAddressManager oscAddressManager)
        {
            this.oscAddressManager = oscAddressManager;

            oscAddressManager.Attach(OscMessages.ErrorOscAddress, OnError);
        }

        private void OnError(OscMessage message)
        {
           // OscMessages.Print
        }
    }
}
