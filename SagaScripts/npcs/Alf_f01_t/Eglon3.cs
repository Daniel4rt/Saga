﻿using System;
using System.Collections.Generic;

using SagaMap;
 
using SagaDB.Actors;
using SagaDB.Items;

namespace Alf_f01_t
{
    public class Eglon3 : Npc
    {
        public override void OnInit()
        {
            MapName = "Alf_f01_t";
            Type = 1250;
            Name = "Eglon3";
            StartX = -24903F;
            StartY = -22578F;
            StartZ = 11107;
            Startyaw = 65061;
            SetScript(823);
            AddButton(Functions.EverydayConversation, new func(OnButton));
        }

        public void OnButton(ActorPC pc)
        {
            NPCChat(pc, 824);
        }

    }
}