using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    // マップ内に敵が10体以上いるなら
    public class WishCount_ManyColonist : WishCountUP
    {
        public override bool CountUP(Map map)
        {
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.IsColonist).ToList(); 
            if (pawns.Count >= 15)
            {
                return true;
            }
            return false;
        }
    }
}
