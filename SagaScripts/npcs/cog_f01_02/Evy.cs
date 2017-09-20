   //////////////////////////////////
  ///        Chii 21/11/07       ///
 ///      Cognito Npc-Pack      ///
//////////////////////////////////

using System;
using System.Collections.Generic;

using SagaMap;
 
using SagaDB.Actors;
using SagaDB.Items;

public class Evy : Npc
{
    public override void OnInit()
    {
        MapName = "cog_f01_02";
        Type = 1189;
        Name = "Evy";
        StartX = 5626F;
        StartY = -26751F;
        StartZ = -71F;
        Startyaw = 14587;
        SetScript(823);
        AddButton(Functions.EverydayConversation, new func(OnButton));
    }
    public void OnButton(ActorPC pc)
    {
        NPCChat(pc, 823);
    }
}
