using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LegacyFairy_Race
{
    [StaticConstructorOnStartup]
    public static class Graphic_LegacyFairy
    {
        public static readonly Texture2D BrillianceCharge_Icon = ContentFinder<Texture2D>.Get("LegacyFairy/UI/Skill/BrillianceCharge");
        public static readonly Texture2D PrayertoDestiny_Icon = ContentFinder<Texture2D>.Get("LegacyFairy/UI/Skill/PrayertoDestiny");
    }
}
