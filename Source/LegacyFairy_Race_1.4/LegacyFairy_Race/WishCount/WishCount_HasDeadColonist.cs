using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    // マップ内に入植者の死体がある
    public class WishCount_HasDeadColonist : WishCountUP
    {
        public override bool CountUP(Map map)
        {
            List<Thing> things = map.spawnedThings.Where(x => x as Corpse != null && ((Corpse)x).InnerPawn.IsColonist).ToList(); 
            if (!things.NullOrEmpty())
            {
                return true;
            }
            return false;
        }
    }
}
