using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaLogin.Packets.Client
{
    public class GameConnection : Packet
    {
        public GameConnection()
        {
            this.size = 104;
            this.offset = 4;
			this.ID = 0x7751;
        }
    }
	
	public override SagaLib.Packet New()
	{
		return (SagaLib.Packet)new SagaLogin.Packets.Client.GameConnection();
	}
	
	public override void Parse(SagaLib.Client client)
	{
		((LoginClient)(client)).OnSendLogin(this);
	}
	
	public override bool isStaticSize()
	{
		false;
	}
	
	public string GetName()
	{
		return this.GetString(4);
	}
	
	public string GetMD5Pass()
	{
		this.size = (ushort)this.data.Length;
		switch (this.data.Length)
		{
			case 104: return this.GetString(40);
			case 106: return this.GetString(86);
			default: return this.GetString(40);
		}
	}

}
