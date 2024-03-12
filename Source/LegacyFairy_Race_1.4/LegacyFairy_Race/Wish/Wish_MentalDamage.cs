using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace LegacyFairy_Race
{
    public class Wish_MentalDamage : WishBase
    {
        // 敵のみ？
        public bool Enemy = false;

        public MentalStateDef MentalState = null;

        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            if (Enemy)
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
            Pawn target = null;
            if (Enemy)
            {
                target = map.mapPawns.AllPawns.Where(x => x.HostileTo(Faction.OfPlayer)).RandomElement();
            }
            else
            {
                target = map.mapPawns.AllPawns.Where(x => x.IsColonist).RandomElement();
            }
            if (target != null)
            {
                target.mindState.mentalStateHandler.TryStartMentalState(MentalState, null, forceWake: true);
            }
            Messages.Message(def.description.Formatted(target), MessageTypeDefOf.PositiveEvent);
        }
    }
}
