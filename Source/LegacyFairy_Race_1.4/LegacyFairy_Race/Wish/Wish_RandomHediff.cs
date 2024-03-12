using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace LegacyFairy_Race
{
    public class Wish_RandomHediffPlayer : WishBase
    {
        public List<HediffDef> Hediffs;

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
                if (Rand.Range(0, 5) < 1)
                {
                    Effecter_BEPCore.BEP_UseSkill_D.Spawn(pawn, map);
                    foreach (HediffDef hediff in Hediffs)
                    {
                        pawn.health.AddHediff(hediff);
                    }
                }
            }
            Messages.Message(def.description, MessageTypeDefOf.PositiveEvent);
        }
    }
}
