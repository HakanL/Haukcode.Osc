using System.Net;

namespace Rug.Osc.Ahoy
{
    public static class AhoyQuery
    {
        public static IAhoyQuery CreateQuery(params IPAddress[] adapterAddress)
        {
            if (adapterAddress.Length == 0)
            {
                // search all adapters
                return new AhoyQueryMultipleAdapters();
            }

            if (adapterAddress.Length > 1)
            {
                // search the range of adapters supplied
                return new AhoyQueryMultipleAdapters(adapterAddress);
            }

            // only search the adapter specified 
            return new AhoyQuerySingleAdapter(adapterAddress[0]);
        }
    }
}
