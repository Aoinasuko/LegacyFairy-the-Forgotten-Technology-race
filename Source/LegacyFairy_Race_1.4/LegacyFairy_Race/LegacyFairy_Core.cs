using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

}
