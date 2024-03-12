using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    public class Wish_RandomDeath : WishBase
    {
        // 敵のみ？
        public bool EnemyOnly = false;

        // 複数体？
        public bool IsMany = false;

        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            if (EnemyOnly)
            {
                if (map.mapPawns.AllPawns.Where(x => x.HostileTo(Faction.OfPlayer)).EnumerableNullOrEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        // 実際に実行される処理
        public override void Run(Map map, WishSelectDef def)
        {
            List<Pawn> pawns = map.mapPawns.AllPawns.Where(x => !EnemyOnly || x.HostileTo(Faction.OfPlayer)).ToList();
            if (IsMany)
            {
                int count = Rand.Range(3, 10);
                for (int i = pawns.Count - 1; i >= 0; i--)
                {
                    Pawn pawn = pawns.ElementAt(i);
                    if (Rand.Range(0, 5) < 1)
                    {
                        Effecter_BEPCore.BEP_UseSkill_D.Spawn(pawn, map);
                        pawn.Kill(null);
                    }
                }
                Messages.Message(def.description, MessageTypeDefOf.PositiveEvent);
            } else
            {
                Pawn pawn = pawns.RandomElement();
                Effecter_BEPCore.BEP_UseSkill_D.Spawn(pawn, map);
                pawn.Kill(null);
                Messages.Message(def.description.Formatted(pawn), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
