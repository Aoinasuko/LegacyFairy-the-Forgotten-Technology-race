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
    public class WishCount_ManyEnemy : WishCountUP
    {
        public override bool CountUP(Map map)
        {
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where(x => x.HostileTo(Faction.OfPlayer)).ToList(); 
            if (pawns.Count >= 10)
            {
                return true;
            }
            return false;
        }
    }
}
