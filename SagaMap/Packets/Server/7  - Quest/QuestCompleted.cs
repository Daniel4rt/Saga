using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class QuestCompleted : Packet
    {
        public QuestCompleted()
        {
            this.data = new byte[8];
            this.ID = 0x0707;
            this.offset = 4;
        }

        public void SetQuestID(uint ID)
        {
            this.PutUInt(ID, 4);
        }

    }
}
