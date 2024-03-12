using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    public class Wish_EventCalc : WishBase
    {
        // 発動するインシデント
        IncidentDef incident = null;

        // 発動回数
        int minStack = 1;
        int maxStack = 1;

        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            return true;
        }

        // 実際に実行される処理
        public override void Run(Map map, WishSelectDef def)
        {
            // インシデントを生成して1秒後に同時発生させる
            int count = Rand.Range(minStack, maxStack + 1);
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(incident.category, map);
            for (int i = 0; i < count; i++) {                
                incident.Worker.TryExecute(parms);
            }
            Messages.Message(def.description, MessageTypeDefOf.PositiveEvent);
        }
    }
}
