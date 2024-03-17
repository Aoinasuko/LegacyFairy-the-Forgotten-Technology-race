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

    [DefOf]
    public static class Sound_LegacyFairy
    {
        public static SoundDef LgcF_Fall;
    }

    [DefOf]
    public static class Effect_LegacyFairy
    {
        public static EffecterDef LgcF_DebilitatedWish;
    }

	[DefOf]
    public static class Thing_LegacyFairy
    {
        public static ThingDef LgcF_TOW_DebilitatedWish;
    }

    [StaticConstructorOnStartup]
    public static class LegacyFairy_Graphic
    {
		public static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();
        public static readonly Material HeadSet = MaterialPool.MatFrom("LegacyFairy/Build/PersonalityRegulator/PersonalityRegulator_Parts", reportFailure: true);
        public static readonly Material HeadSet_Open = MaterialPool.MatFrom("LegacyFairy/Build/PersonalityRegulator/PersonalityRegulator_Open", reportFailure: true);
		public static readonly Material Mat_ShotGear_A = MaterialPool.MatFrom("LegacyFairy/UI/TOW/ShotGear_A", ShaderDatabase.CutoutComplex);
        public static readonly Material Mat_ShotGear_B = MaterialPool.MatFrom("LegacyFairy/UI/TOW/ShotGear_B", ShaderDatabase.CutoutComplex);
		public static readonly Material Shield = MaterialPool.MatFrom("LegacyFairy/UI/Mote/ElectricShock", ShaderDatabase.MoteGlow);
        public static readonly MaterialPropertyBlock ShieldBlock = new MaterialPropertyBlock();
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
