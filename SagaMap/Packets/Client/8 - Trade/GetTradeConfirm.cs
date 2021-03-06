using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Client
{
    public class GetTradeConfirm : Packet // 0x0806
    {
       public GetTradeConfirm()
       {
           this.size = 4;
       }
        
       public override SagaLib.Packet New()
       {
           return (SagaLib.Packet)new SagaMap.Packets.Client.GetTradeConfirm();
       }

       public override void Parse(SagaLib.Client client)
       {
           ((MapClient)(client)).OnTradeConfirm(this);
       }
    }
}