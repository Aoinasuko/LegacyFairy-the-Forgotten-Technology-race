﻿using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace LegacyFairy_Race
{

    [StaticConstructorOnStartup]
    static class LegacyFairy_Harmony
    {
        static LegacyFairy_Harmony()
        {
            var harmony = new Harmony("BEP.LegacyFairy");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(BodyPartDef), "GetMaxHealth")]
    static class GetMaxHealth_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref float __result, ref Pawn pawn)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("LgcF_SoulDepletion"));
            if (hediff != null)
            {
                float liferate = Math.Max(1f - hediff.Severity, 0.1f);
                __result = (int)(__result * liferate);
                if (__result < 1)
                {
                    __result = 1;
                }
            }
        }
    }
}
