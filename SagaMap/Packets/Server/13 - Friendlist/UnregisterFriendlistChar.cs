using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{

    public class UnregisterFriendlistChar : Packet
    {

        public UnregisterFriendlistChar()
        {
            this.data = new byte[39];
            this.ID = 0x1302;
            this.offset = 4;
        }

        public void SetName(string name)
        {
            PutString(name, 4);
        }
  
        public void SetReason(byte reason)
        {
            PutByte(reason, 38);
        }

    }
}

