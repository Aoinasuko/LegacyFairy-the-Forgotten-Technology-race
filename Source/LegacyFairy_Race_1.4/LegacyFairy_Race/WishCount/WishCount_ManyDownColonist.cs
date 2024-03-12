using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    // マップ内にダウンしている入植者が3人以上居るなら
    public class WishCount_ManyDownColonist : WishCountUP
    {
        public override bool CountUP(Map map)
        {
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.IsColonist && x.Downed).ToList(); 
            if (pawns.Count >= 3)
            {
                return true;
            }
            return false;
        }
    }
}
