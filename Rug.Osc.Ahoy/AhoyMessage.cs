﻿using System;
using System.Net;

namespace Rug.Osc.Ahoy
{
    public static class AhoyMessage
    {
        /// <summary>
        /// Creates the ahoy broadcast.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="namespace">The namespace.</param>
        /// <returns>OscMessage.</returns>
        /// <autogeneratedoc />
        /// TODO Edit XML Comment Template for CreateAhoyBroadcast
        public static OscMessage CreateAhoyBroadcast(IPAddress sourceAddress, string @namespace = null)
        {
            return CreateAhoyBroadcast_Inner(GetAhoyAddress(@namespace), sourceAddress);
        }

        /// <summary>
        /// Creates the ahoy response.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="sendPort">The send port.</param>
        /// <param name="listenPort">The listen port.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="namespace">The namespace.</param>
        /// <returns>OscMessage.</returns>
        /// <autogeneratedoc />
        /// TODO Edit XML Comment Template for CreateAhoyResponse
        public static OscMessage CreateAhoyResponse(IPAddress sourceAddress, int sendPort, int listenPort, string descriptor, string @namespace = null)
        {
            return CreateAhoyResponse_Inner(GetAhoyAddress(@namespace), sourceAddress, sendPort, listenPort, descriptor);
        }

        /// <summary>
        /// Is the address of a message a valid /ahoy address
        /// </summary>
        /// <param descriptor="message">A OscMessage</param>
        /// <returns>True if the message has a valid /ahoy address</returns>
        public static bool IsAddressValid(OscMessage message)
        {
            if (message == null)
            {
                return false;
            }

            if (message.Address.Length == AhoyConstants.Address.Length)
            {
                return message.Address.Equals(AhoyConstants.Address, StringComparison.InvariantCulture);
            }

            return message.Address.StartsWith(AhoyConstants.Address + "/", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Determines whether [is valid response] [the specified message].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if [is valid response] [the specified message]; otherwise, <c>false</c>.</returns>
        /// <autogeneratedoc />
        /// TODO Edit XML Comment Template for IsValidResponse
        public static bool IsValidResponse(OscMessage message)
        {
            if (IsAddressValid(message) == false)
            {
                return false;
            }

            if (message.Count < 4)
            {
                return false;
            }

            if (!(message[0] is string))
            {
                return false;
            }

            if (!(message[1] is int))
            {
                return false;
            }

            if (!(message[2] is int))
            {
                return false;
            }

            if (!(message[3] is string))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries the get ahoy service information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="serviceInfo">The service information.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <autogeneratedoc />
        /// TODO Edit XML Comment Template for TryGetAhoyServiceInfo
        public static bool TryGetAhoyServiceInfo(IPAddress adapterAddress, OscMessage message, int serviceExpiryPeriod, out AhoyServiceInfo serviceInfo)
        {
            serviceInfo = default(AhoyServiceInfo);

            IPAddress address;
            int listenPort, sendPort;
            string descriptor;

            if (TryGetResponseDetails(message, out address, out listenPort, out sendPort, out descriptor) == false)
            {
                return false;
            }

            serviceInfo = new AhoyServiceInfo(adapterAddress, address, listenPort, sendPort, descriptor, GetNamespaceString(message.Address), GetPropertiesObjects(message), serviceExpiryPeriod);

            return true;
        }

        /// <summary>
        /// Get the desired return IP address and port from an /ahoy message.
        /// </summary>
        /// <param descriptor="message"></param>
        /// <param descriptor="address"></param>
        /// <returns>True if a IP address was present.</returns>
        public static bool TryGetIPAddress(OscMessage message, out IPAddress address)
        {
            address = default(IPAddress);

            if (message == null)
            {
                return false;
            }

            if (message.Count < 1)
            {
                return false;
            }

            // the first argument of any /ahoy message should be the desired return IP address
            if (IPAddress.TryParse(message[0].ToString(), out address) == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the desired return IP address and port from an /ahoy message.
        /// </summary>
        /// <param descriptor="message"></param>
        /// <param descriptor="address"></param>
        /// <param descriptor="port"></param>
        /// <returns>True if a IP address was present.</returns>
        public static bool TryGetIPAddressAndPort(OscMessage message, out IPAddress address, out int port)
        {
            address = default(IPAddress);
            port = -1;

            if (message == null)
            {
                return false;
            }

            if (message.Count < 2)
            {
                return false;
            }

            // the first argument of any /ahoy message should be the desired return IP address
            if (IPAddress.TryParse(message[0].ToString(), out address) == false)
            {
                return false;
            }

            if ((message[1] is int) == false)
            {
                return false;
            }

            port = (int)message[1];

            return true;
        }

        /// <summary>
        /// Tries the get response details.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="address">The address.</param>
        /// <param name="listenPort">The listen port.</param>
        /// <param name="sendPort">The send port.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns><c>true</c> if the , <c>false</c> otherwise.</returns>
        /// <autogeneratedoc />
        /// TODO Edit XML Comment Template for TryGetResponseDetails
        public static bool TryGetResponseDetails(OscMessage message, out IPAddress address, out int listenPort, out int sendPort, out string descriptor)
        {
            descriptor = null;
            address = default(IPAddress);
            listenPort = 0;
            sendPort = 0;

            if (IsValidResponse(message) == false)
            {
                return false;
            }

            if (IPAddress.TryParse(message[0] as string, out address) == false)
            {
                return false;
            }

            listenPort = (int)message[1];
            sendPort = (int)message[2];

            descriptor = message[3] as string;

            return true;
        }

        private static OscMessage CreateAhoyBroadcast_Inner(string fullAddress, IPAddress sourceAddress)
        {
            OscMessage message = new OscMessage(fullAddress, sourceAddress.ToString());

            return message;
        }

        private static OscMessage CreateAhoyResponse_Inner(string fullAddress, IPAddress sourceAddress, int sendPort, int listenPort, string descriptor)
        {
            OscMessage message = new OscMessage(fullAddress, sourceAddress.ToString(), sendPort, listenPort, descriptor);

            return message;
        }

        private static string GetNamespaceString(string fullAddress)
        {
            if (fullAddress.Length > AhoyConstants.Address.Length + 1)
            {
                return fullAddress.Substring(AhoyConstants.Address.Length + 1);
            }
            else
            {
                return string.Empty;
            }
        }

        private static object[] GetPropertiesObjects(OscMessage message)
        {
            if (message.Count <= 4)
            {
                return new object[0];
            }

            object[] properties = new object[message.Count - 4];

            Array.Copy(message.ToArray(), 4, properties, 0, message.Count - 4);

            return properties;
        }

        public static string GetAhoyAddress(string @namespace = null)
        {
            if (string.IsNullOrEmpty(@namespace) == true)
            {
                return AhoyConstants.Address;
            }
            else
            {
                return AhoyConstants.Address + "/" + @namespace;
            }
        }
    }
}