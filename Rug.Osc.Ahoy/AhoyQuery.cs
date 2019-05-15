using System.Net;

namespace Rug.Osc.Ahoy
{
    public static class AhoyQuery
    {
        public static IAhoyQuery CreateQuery(params IPAddress[] adapterAddress)
        {
            return CreateQuery(null, adapterAddress);
        }

        public static IAhoyQuery CreateQuery(string @namespace, params IPAddress[] adapterAddress)
        {
            if (adapterAddress.Length == 0)
            {
                // search all adapters
                return new AhoyQueryMultipleAdapters(@namespace);
            }

            if (adapterAddress.Length > 1)
            {
                // search the range of adapters supplied
                return new AhoyQueryMultipleAdapters(@namespace, adapterAddress);
            }

            // only search the adapter specified
            return new AhoyQuerySingleAdapter(@namespace, adapterAddress[0]);
        }

        public static IAhoyQuery CreateQuery(int serviceExpiryPeriod, params IPAddress[] adapterAddress)
        {
            return CreateQuery(null, serviceExpiryPeriod, adapterAddress);
        }

        public static IAhoyQuery CreateQuery(string @namespace, int serviceExpiryPeriod, params IPAddress[] adapterAddress)
        {
            if (adapterAddress.Length == 0)
            {
                // search all adapters
                return new AhoyQueryMultipleAdapters(@namespace, serviceExpiryPeriod);
            }

            if (adapterAddress.Length > 1)
            {
                // search the range of adapters supplied
                return new AhoyQueryMultipleAdapters(@namespace, serviceExpiryPeriod, adapterAddress);
            }

            // only search the adapter specified
            return new AhoyQuerySingleAdapter(@namespace, adapterAddress[0], serviceExpiryPeriod);
        }
    }
}