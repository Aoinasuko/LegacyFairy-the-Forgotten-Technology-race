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
    public class Wish_PlantGrowth : WishBase
    {
        bool Death = false;

        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            return true;
        }

        // 実際に実行される処理
        public override void Run(Map map, WishSelectDef def)
        {
            List<Thing> plants = map.spawnedThings.Where(x => x as Plant != null).ToList();
            for (int i = plants.Count - 1; i >= 0; i--)
            {
                Plant plant = (Plant)plants.ElementAt(i);
                Effecter_BEPCore.BEP_UseSkill_D.Spawn(plant, map);
                if (Death)
                {
                    // 植物即死
                    plant.Kill();
                } else
                {
                    // 植物成長
                    plant.Growth = 1f;
                }
            }
            Messages.Message(def.description, MessageTypeDefOf.PositiveEvent);
        }
    }
}
