using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    public class Wish_SummonThing : WishBase
    {
        // 召喚するアイテム候補
        List<ThingDef> thingList = null;

        // 材質候補
        List<ThingDef> stuffList = null;

        // スタック数
        int minStack = 1;
        int maxStack = 1;

        // 品質修正
        QualityCategory qualityfix = 0;

        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            return true;
        }

        // 実際に実行される処理
        public override void Run(Map map, WishSelectDef def)
        {
            // 召喚するアイテムをランダムで選ぶ
            ThingDef summonthing = thingList.RandomElement();
            ThingDef summonstuff = null;
            if (!stuffList.NullOrEmpty())
            {
                summonstuff = stuffList.RandomElement();
            }
            int count = Rand.Range(minStack, maxStack + 1);
            Thing thing = ThingMaker.MakeThing(summonthing, summonstuff);
            thing.stackCount = count;
            if (thing.TryGetComp<CompQuality>() != null)
            {
                thing.TryGetComp<CompQuality>().SetQuality(qualityfix, ArtGenerationContext.Outsider);
            }
            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            List<Thing> things = new List<Thing>();
            things.Add(thing);
            ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
            activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(things);
            activeDropPodInfo.openDelay = 180;
            activeDropPodInfo.leaveSlag = false;
            DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo);
            Messages.Message(def.description.Formatted(thing), thing, MessageTypeDefOf.PositiveEvent);
        }
    }
}
