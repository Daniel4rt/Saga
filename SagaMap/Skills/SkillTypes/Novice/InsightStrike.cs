using System;
using System.Collections.Generic;
using System.Text;

using SagaDB.Actors;
using SagaLib;
namespace SagaMap.Skills.SkillTypes
{
    public static class InsightStrike
    {
        const SkillIDs baseID = SkillIDs.InsightStrike;
        public static void Proc(ref Actor sActor, ref Actor dActor,ref Map.SkillArgs args)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                ActorEventHandlers.PC_EventHandler eh = (ActorEventHandlers.PC_EventHandler)pc.e;
                if (!SkillHandler.CheckSkillSP(pc, args.skillID) || pc.LP == 0)
                {
                    SkillHandler.SetSkillFailed(ref args);
                    return;
                }
                args.damage = 0;
                args.isCritical = SkillHandler.CalcCrit(sActor, dActor, args, SkillHandler.AttackType.Physical);
                if (args.isCritical != Map.SkillArgs.AttackResult.Miss && args.isCritical != Map.SkillArgs.AttackResult.Block)
                {
                    args.damage = CalcDamage(sActor, dActor, args);                    
                } 
                pc.LP = 0;
                eh.C.SendCharStatus(0);
            }
            SkillHandler.PhysicalAttack(ref sActor, ref dActor, args.damage, SkillHandler.AttackElements.NEUTRAL, ref args); 
        }

        private static uint CalcDamage(Actor sActor,Actor dActor,Map.SkillArgs args)
        {
            ActorPC pc = (ActorPC)sActor;
            byte level = (byte)(args.skillID - baseID + 1);
            int dmg = 0;
            switch (pc.LP)
            {
                case 1:
                    dmg = 10 + level * 10;
                    break;
                case 2:
                    dmg = 25 + level * 15;
                    break;
                case 3:
                    dmg = 40 + level * 20;
                    break;
                case 4:
                    dmg = 55 + level * 25;
                    break;
                case 5:
                    dmg = 70 + level * 30;
                    break;
            }
            return (uint)(pc.BattleStatus.atk + dmg);
        }

        

    }
}
