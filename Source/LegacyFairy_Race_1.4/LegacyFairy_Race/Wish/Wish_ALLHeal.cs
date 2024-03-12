using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    public class Wish_AllHeal : WishBase
    {
        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            return true;
        }

        // 実際に実行される処理
        public override void Run(Map map, WishSelectDef def)
        {
            List<Pawn> pawns = map.mapPawns.AllPawns.Where(x => x.IsColonist).ToList();
            for (int i = pawns.Count - 1; i >= 0; i--)
            {
                Pawn pawn = pawns.ElementAt(i);
                Effecter_BEPCore.BEP_UseSkill_D.Spawn(pawn, map);
                Util_Heal.HealInjury(pawn, 9999);
                Util_Heal.HealFirstMissingBodyPart(pawn);
                Util_Heal.HealFirstDisease(pawn);
            }
            Messages.Message(def.description, MessageTypeDefOf.PositiveEvent);
        }
    }

    public static class Util_Heal
    {
        // 傷と傷跡を治癒
        public static void HealInjury(Pawn pawn, int amount)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                Hediff_Injury hediff_Injury = hediffs[i] as Hediff_Injury;
                if (hediff_Injury != null && hediff_Injury.Visible && hediff_Injury.def.everCurableByItem)
                {
                    hediff_Injury.Heal(amount);
                }
            }
            return;
        }

        public static void HealFirstMissingBodyPart(Pawn pawn)
        {
            List<Hediff_MissingPart> hediffs = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                pawn.health.RemoveHediff(hediffs[i]);
            }
            return;
        }

        public static void HealFirstDisease(Pawn pawn)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                if (!hediffs[i].Visible || !hediffs[i].def.everCurableByItem || hediffs[i].FullyImmune())
                {
                    continue;
                }
                if (hediffs[i].def.lethalSeverity >= 0f)
                {
                    pawn.health.RemoveHediff(hediffs[i]);
                }
            }
            return;
        }
    }
}
