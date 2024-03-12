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
    [StaticConstructorOnStartup]
    public class Command_Select : Command
    {
        public Pillar_Cast pillar;

        static List<ThingDef> thingDefs;

        public Command_Select(Pillar_Cast pillar)
        {
            this.pillar = pillar;
            if (thingDefs.NullOrEmpty())
            {
                // 一覧取得
                thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff).ToList();
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            
            foreach (ThingDef thingDef in thingDefs)
            {
                list.Add(new FloatMenuOption(thingDef.label, delegate
                {
                    pillar.selectStuff = thingDef;
                    defaultLabel = thingDef.label;
                    icon = thingDef.uiIcon;
                }, thingDef.uiIcon, Color.white));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
