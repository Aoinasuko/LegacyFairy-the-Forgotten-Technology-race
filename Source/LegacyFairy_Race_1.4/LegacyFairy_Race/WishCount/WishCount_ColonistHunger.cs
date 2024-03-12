using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    // 餓死中の入植者が居るなら
    public class WishCount_ColonistHunger : WishCountUP
    {
        public override bool CountUP(Map map)
        {
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.needs.food?.Starving ?? false).ToList();
            if (!pawns.NullOrEmpty())
            {
                return true;
            }
            return false;
        }
    }
}
