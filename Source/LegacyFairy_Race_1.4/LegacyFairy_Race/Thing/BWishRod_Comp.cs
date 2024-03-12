using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{

    public class CompTargetable_Self : CompTargetable
    {
        protected override bool PlayerChoosesTarget => false;

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetSelf = true,
                canTargetPawns = true,
                canTargetBuildings = false,
                validator = ((TargetInfo x) => BaseTargetValidator(x.Thing))
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }
    }

    public class CompTargetEffect_BWishRod : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Map map = user.Map;
            // 現在発動可能な願いを入れる
            List<WishSelectDef> wishList = new List<WishSelectDef>();
            foreach(WishSelectDef sel in DefDatabase<WishSelectDef>.AllDefsListForReading)
            {
                if (sel.WishCalc.Test(map)) {
                    wishList.Add(sel);
                    // もし確率アップの条件を満たしているなら追加で候補に入れる
                    if (sel.WishCountUP != null)
                    {
                        if (sel.WishCountUP.CountUP(map))
                        {
                            for (int i = 0; i < sel.CountUP; i++)
                            {
                                wishList.Add(sel);
                            }
                        }
                    }
                }
            }
            // ランダムで願いを選択して発動
            WishSelectDef start = wishList.RandomElement();
            start.WishCalc.Run(map, start);
            parent.SplitOff(1).Destroy();
        }
    }
}
