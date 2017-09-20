using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Login.Get
{
    /// <summary>
    /// Client sending a GUID to the server.
    /// </summary>
    public class SendGUID : Packet
    {
        /// <summary>
        /// Create a SendGUID packet.
        /// </summary>
        public SendGUID()
        {
            this.size = 24;
            this.offset = 4;
        }


        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Login.Get.SendGUID();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginSession)(client)).OnSendGUID(this);
        }


    }
}