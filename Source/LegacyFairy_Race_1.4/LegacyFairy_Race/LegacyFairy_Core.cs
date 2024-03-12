using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using Verse;

namespace LegacyFairy_Race
{
    [DefOf]
    public static class FleshType_LegacyFairy
    {
        public static FleshTypeDef LgcF_Fairy;
    }

    [StaticConstructorOnStartup]
    public static class LegacyFairy_Graphic
    {
        public static readonly Material HeadSet = MaterialPool.MatFrom("LegacyFairy/Build/PersonalityRegulator/PersonalityRegulator_Parts", reportFailure: true);
        public static readonly Material HeadSet_Open = MaterialPool.MatFrom("LegacyFairy/Build/PersonalityRegulator/PersonalityRegulator_Open", reportFailure: true);
    }

    public static class DebugTools_Wish
    {
        [DebugAction("ModAction", null, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 1000)]
        private static List<DebugActionNode> CalcWish()
        {
            List<DebugActionNode> list = new List<DebugActionNode>();
            foreach (WishSelectDef item in DefDatabase<WishSelectDef>.AllDefs.OrderBy((WishSelectDef wish) => wish.defName))
            {
                WishSelectDef wishDef = item;
                list.Add(new DebugActionNode(wishDef.defName, DebugActionType.Action)
                {
                    action = delegate
                    {
                        if (wishDef.WishCalc.Test(Find.CurrentMap))
                        {
                            wishDef.WishCalc.Run(Find.CurrentMap, wishDef);
                        }
                    }
                });
            }
            return list;
        }
    }

}
