//Comment this out to deactivate the dead lock check!
//#define DeadLockCheck

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using SagaLib;


namespace SagaGateway 
{
    sealed class GatewayClientManager : ClientManager 
    {
        public Dictionary<uint, GatewayClient> clients;

        private uint count = 1;
        public Thread check;
        GatewayClientManager()
        {
            this.clients = new Dictionary<uint, GatewayClient>();
            this.commandTable = new Dictionary<ushort, Packet>();

            //here for packets
            this.commandTable.Add(0x0101, new Packets.Client.SendKey());
            this.commandTable.Add(0x0102, new Packets.Client.SendGUID());
            this.commandTable.Add(0x0104, new Packets.Client.SendIdentify());
            this.commandTable.Add(0x0105, new Packets.Client.RequestSession());


            this.commandTable.Add(0xFFFF, new Packets.Client.SendUniversal());

            
            this.waitressQueue = new AutoResetEvent(false);
            this.waitressHasFinished = new ManualResetEvent(false);
            this.waitingWaitressesCount = 0;
            this.waitressCountLock = new Object();

            this.packetCoordinator = new Thread(new ThreadStart(this.packetCoordinationLoop));
            this.packetCoordinator.Start();

            //deadlock check
            check = new Thread(new ThreadStart(this.checkCriticalArea));
#if DeadLockCheck
            check.Start();
#endif
        }

        public uint GetNextSessionID()
        {
            return count++;
        }

        public static GatewayClientManager Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly GatewayClientManager instance = new GatewayClientManager();
        }


        /// <summary>
        /// Connects new clients
        /// </summary>
        public override void NetworkLoop(int maxNewConnections)
        {
            for (int i = 0; listener.Pending() && i < maxNewConnections; i++)
            {
                Socket sock = listener.AcceptSocket();
                string ip = sock.RemoteEndPoint.ToString().Substring(0, sock.RemoteEndPoint.ToString().IndexOf(':'));
                if (Gateway.lcfg.Banned_IP.Contains(ip))
                {
                    Logger.ShowWarning("Conncetion from banned IP:" + ip + " rejected!");
                    sock.Close();
                }
                else
                {
                    Logger.ShowInfo("New client from: " + sock.RemoteEndPoint.ToString(), null);
                    GatewayClient newClient = new GatewayClient(sock, this.commandTable);
                }
            }
        }

        public override void OnClientDisconnect(Client client_t)
        {
            GatewayClient client = (GatewayClient)client_t;

            this.clients.Remove(client.SessionID);
        }

    }
}
