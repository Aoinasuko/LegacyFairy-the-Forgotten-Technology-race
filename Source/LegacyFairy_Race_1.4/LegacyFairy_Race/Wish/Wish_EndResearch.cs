using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LegacyFairy_Race
{
    public class Wish_EndResearch : WishBase
    {
        bool IsMany = false;

        // この条件を満たしてる場合候補に選ばれる
        public override bool Test(Map map)
        {
            if (IsMany)
            {
                return Find.ResearchManager.AnyProjectIsAvailable;
            }
            return Find.ResearchManager.currentProj != null;
        }

        // 実際に実行される処理
        public override void Run(Map map, WishSelectDef def)
        {
            Messages.Message(def.description, MessageTypeDefOf.PositiveEvent);
            if (IsMany)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Find.ResearchManager.AnyProjectIsAvailable)
                    {
                        ResearchProjectDef ResearchDef = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where(x => x.CanStartNow).RandomElement();
                        Find.ResearchManager.FinishProject(ResearchDef, true);
                        Messages.Message("ResearchFinished".Translate(ResearchDef.label), MessageTypeDefOf.PositiveEvent);
                    } else
                    {
                        break;
                    }
                }
            } else
            {
                Find.ResearchManager.FinishProject(Find.ResearchManager.currentProj, true);
            }         
        }
    }
}
