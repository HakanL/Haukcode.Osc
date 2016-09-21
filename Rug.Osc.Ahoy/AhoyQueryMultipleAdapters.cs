using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace Rug.Osc.Ahoy
{
    internal class AhoyQueryMultipleAdapters : IAhoyQuery
    {
        private readonly List<AhoyServiceInfo> ahoyServiceInfoList = new List<AhoyServiceInfo>();
        private readonly object ahoyServiceInfoListSyncLock = new object();
        private readonly bool OnlyIpV4 = false;

        private readonly List<IPAddress> potentialAdapters = new List<IPAddress>();
        private readonly List<IAhoyQuery> queriesList = new List<IAhoyQuery>();
        private readonly object queriesListSyncLock = new object();

        public int Count { get { return ahoyServiceInfoList.Count; } }

        public string Namespace { get; private set; } 

        public AhoyServiceInfo this[int index] { get { return ahoyServiceInfoList[index]; } }

        public event OscMessageEvent AnyReceived;

        public event OscMessageEvent MessageReceived;

        public event OscMessageEvent MessageSent;

        public event AhoyServiceEvent ServiceDiscovered;

        public event AhoyServiceEvent ServiceExpired;

        public AhoyQueryMultipleAdapters(string @namespace, params IPAddress[] adapterAddress)
        {
            Namespace = @namespace; 

            potentialAdapters.AddRange(adapterAddress);
        }

        public void BeginSearch(int sendInterval = 100)
        {
            EndSearch();

            lock (queriesListSyncLock)
            {
                queriesList.Clear();

                lock (ahoyServiceInfoListSyncLock)
                {
                    ahoyServiceInfoList.Clear();
                }

                {
                    IAhoyQuery query = CreateQuery(null, IPAddress.Loopback);

                    if (query != null)
                    {
                        queriesList.Add(query);
                    }
                }

                try
                {
                    foreach (NetworkInterface @interface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        var ipProps = @interface.GetIPProperties();

                        foreach (var ip in ipProps.UnicastAddresses)
                        {
                            if (potentialAdapters.Count > 0 && potentialAdapters.Contains(ip.Address) == false)
                            {
                                continue;
                            }

                            if (((ip.Address.AddressFamily != AddressFamily.InterNetwork) &&
                                (OnlyIpV4 == false && ip.Address.AddressFamily == AddressFamily.InterNetworkV6)))
                            {
                                continue;
                            }

                            IAhoyQuery query = CreateQuery(@interface, ip.Address);

                            if (query == null)
                            {
                                continue;
                            }

                            queriesList.Add(query);
                        }
                    }
                }
                catch (EntryPointNotFoundException ex)
                {
                    foreach (IPAddress ipAddress in LocalIPAddresses())
                    {
                        if (potentialAdapters.Count > 0 && potentialAdapters.Contains(ipAddress) == false)
                        {
                            continue;
                        }

                        if (((ipAddress.AddressFamily != AddressFamily.InterNetwork) &&
                            (OnlyIpV4 == false && ipAddress.AddressFamily == AddressFamily.InterNetworkV6)))
                        {
                            continue;
                        }

                        IAhoyQuery query = CreateQuery(null, ipAddress);

                        if (query == null)
                        {
                            continue;
                        }

                        queriesList.Add(query);
                    }
                }

                foreach (AhoyQuerySingleAdapter query in queriesList)
                {
                    query.BeginSearch(sendInterval);
                }
            }
        }

        private IPAddress[] LocalIPAddresses()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList;
        }

        public void Dispose()
        {
            EndSearch();
        }

        public void EndSearch()
        {
            lock (queriesListSyncLock)
            {
                foreach (AhoyQuerySingleAdapter query in queriesList)
                {
                    query.EndSearch();
                }

                queriesList.Clear();
            }
        }

        public IEnumerator<AhoyServiceInfo> GetEnumerator()
        {
            return ahoyServiceInfoList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (ahoyServiceInfoList as System.Collections.IEnumerable).GetEnumerator();
        }

        public void Search(int sendInterval = 100, int timeout = 500)
        {
            try
            {
                BeginSearch(sendInterval);

                Thread.CurrentThread.Join(timeout);
            }
            finally
            {
                EndSearch();
            }
        }

        private IAhoyQuery CreateQuery(NetworkInterface @interface, IPAddress adapterAddress)
        {
            try
            {
                IAhoyQuery query = AhoyQuery.CreateQuery(Namespace, adapterAddress);

                query.AnyReceived += OnAnyReceived;

                query.MessageSent += OnMessageSent;

                query.MessageReceived += OnMessageReceived;

                query.ServiceDiscovered += OnServiceDiscovered;

                query.ServiceExpired += OnServiceExpired;

                return query;
            }
            catch
            {
                return null;
            }
            finally
            {
            }
        }

        private void OnServiceExpired(AhoyServiceInfo serviceInfo)
        {
            lock (ahoyServiceInfoListSyncLock)
            {
                ahoyServiceInfoList.Remove(serviceInfo);
            }

            ServiceExpired?.Invoke(serviceInfo);
        }

        private void OnAnyReceived(OscMessage message)
        {
            OscMessageEvent @event = AnyReceived;

            if (@event == null)
            {
                return;
            }

            @event(message);
        }

        private void OnMessageReceived(OscMessage message)
        {
            OscMessageEvent @event = MessageReceived;

            if (@event == null)
            {
                return;
            }

            @event(message);
        }

        private void OnMessageSent(OscMessage message)
        {
            OscMessageEvent @event = MessageSent;

            if (@event == null)
            {
                return;
            }

            @event(message);
        }

        private void OnServiceDiscovered(AhoyServiceInfo serviceInfo)
        {
            lock (ahoyServiceInfoListSyncLock)
            {
                ahoyServiceInfoList.Add(serviceInfo);
            }

            ServiceDiscovered?.Invoke(serviceInfo);
        }
    }
}