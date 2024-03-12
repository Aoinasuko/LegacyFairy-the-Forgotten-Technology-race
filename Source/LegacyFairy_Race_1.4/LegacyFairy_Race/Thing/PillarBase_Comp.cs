using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace LegacyFairy_Race
{
    /// <summary>
    /// 柱の効果
    /// </summary>
    public class CompProperties_Pillar : CompProperties
    {
        // ポーンをターゲット出来るか
        public bool canTargetPawn = false;
        
        // 物体をターゲット出来るか
        public bool canTargetThing = false;

        // レンジか？
        public bool isRange = false;

        [MustTranslate]
        public string commandLabel;

        [MustTranslate]
        public string commandDesc;

        public CompProperties_Pillar()
        {
            compClass = typeof(PillarBase_Comp);
        }
    }

    public abstract class PillarBase_Comp : ThingComp
    {
        public CompProperties_Pillar Props => (CompProperties_Pillar)props;

        // WPが足りるか？
        public bool needPower(out String reason)
        {
            Comp_WishPower compWP = parent.TryGetComp<Comp_WishPower>();
            if (compWP != null)
            {
                reason = "LegacyFairy.UI.disabled_LowWP".Translate();
                return compWP.Props.usePower <= compWP.nowPower;
            }
            reason = "";
            return false;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!this.parent.Faction.IsPlayer)
            {
                yield break;
            }
            if (Props.canTargetPawn == true || Props.canTargetThing == true)
            {
                Command_Target Gizmo = new Command_Target
                {
                    defaultLabel = Props.commandLabel,
                    defaultDesc = Props.commandDesc,
                    icon = Graphic_LegacyFairy.UsePillar_Icon,
                    disabled = !needPower(out String reason),
                    disabledReason = reason,
                    targetingParams = new TargetingParameters()
                    {
                        canTargetSelf = false,
                        canTargetPawns = Props.canTargetPawn,
                        canTargetLocations = Props.canTargetThing,
                        thingCategory = ThingCategory.None,
                        canTargetBuildings = false,
                        validator = delegate (TargetInfo targ)
                        {
                            if (parent.Position.DistanceTo(targ.Cell) > parent.def.specialDisplayRadius)
                            {
                                return false;
                            }
                            if (Props.isRange)
                            {
                                DrawRadiusRing(targ.Cell, 5.9f, UnityEngine.Color.white, false, null);
                            }
                            return true;
                        },
                    },
                    action = delegate (LocalTargetInfo target)
                    {
                        commandAction(target);
                    },
                };
                yield return Gizmo;
            } else
            {
                Command_Action Gizmo = new Command_Action
                {
                    defaultLabel = Props.commandLabel,
                    defaultDesc = Props.commandDesc,
                    icon = Graphic_LegacyFairy.UsePillar_Icon,
                    disabled = !needPower(out String reason),
                    disabledReason = reason,
                    action = delegate()
                    {
                        commandAction();
                    },
                };
                yield return Gizmo;
            }
        }

        public virtual void commandAction()
        {
            Effecter_BEPCore.BEP_UseSkill_E.Spawn(parent, parent.Map);
            Comp_WishPower compWP = parent.TryGetComp<Comp_WishPower>();
            if (compWP != null)
            {
                compWP.Used(compWP.Props.usePower, null);
            }
        }

        public virtual void commandAction(LocalTargetInfo target)
        {
            Effecter_BEPCore.BEP_UseSkill_E.Spawn(target.Cell, parent.Map);
            Comp_WishPower compWP = parent.TryGetComp<Comp_WishPower>();
            if (compWP != null)
            {
                compWP.Used(compWP.Props.usePower, null);
            }
        }

        public static void DrawRadiusRing(IntVec3 center, float radius, UnityEngine.Color color, bool nonwall, Func<IntVec3, bool> predicate = null)
        {
            if (radius > GenRadial.MaxRadialPatternRadius)
            {
                return;
            }
            List<IntVec3> ringDrawCells = new List<IntVec3>();
            ringDrawCells.Clear();
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = center + GenRadial.RadialPattern[i];
                if (predicate == null || predicate(intVec))
                {
                    if (nonwall || GenSight.LineOfSight(center, intVec, Find.CurrentMap))
                    {
                        ringDrawCells.Add(intVec);
                    }
                }
            }
            GenDraw.DrawFieldEdges(ringDrawCells, color);
        }

    }

    // 守護の柱
    public class Pillar_Protection : PillarBase_Comp
    {
        public override void commandAction(LocalTargetInfo target)
        {
            base.commandAction(target);
            target.Pawn.health.AddHediff(HediffDef.Named("LgcF_Protection"));
        }
    }

    // 重力の柱
    public class Pillar_Gravity : PillarBase_Comp
    {
        public override void commandAction(LocalTargetInfo target)
        {
            base.commandAction(target);
            target.Pawn.health.AddHediff(HediffDef.Named("LgcF_Gravity"));
        }
    }

    // 消滅の柱
    public class Pillar_Disposition : PillarBase_Comp
    {
        public override void commandAction(LocalTargetInfo target)
        {
            base.commandAction(target);
            GenExplosion.DoExplosion(target.Cell, parent.Map, 5.9f, DamageDefOf.Bomb, parent, 30, 5f, null, null, null, null, null, 1f, 1, null, false, null, 0, 0, 0, false);
        }
    }

    // 変換の柱
    public class Pillar_Cast : PillarBase_Comp
    {
        public ThingDef selectStuff = ThingDefOf.WoodLog;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref selectStuff, "selectStuff");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (!this.parent.Faction.IsPlayer)
            {
                yield break;
            }
            Command_Select Gizmo = new Command_Select(this)
            {
                defaultLabel = selectStuff.label,
                icon = selectStuff.uiIcon
            };
            yield return Gizmo;
        }

        public override void commandAction(LocalTargetInfo target)
        {
            List<Thing> things = target.Cell.GetThingList(parent.Map).Where(x => x as Pawn == null).ToList();

            if (!things.NullOrEmpty())
            {
                float count = 0f;

                foreach (Thing thing in things)
                {
                    count += thing.MarketValue * thing.stackCount;
                }

                if (selectStuff != null)
                {
                    if (selectStuff.BaseMarketValue <= count)
                    {
                        // アイテム変換処理
                        int stack = Math.Max(1, (int)(count / selectStuff.BaseMarketValue));
                        for (int i = things.Count - 1; i >= 0; i--)
                        {
                            Thing thing = things.ElementAt(i);
                            thing.Destroy();
                        }
                        Thing summon = GenSpawn.Spawn(selectStuff, target.Cell, parent.Map);
                        summon.stackCount = Math.Min(selectStuff.stackLimit, stack);
                        base.commandAction(target);
                    } else
                    {
                        Messages.Message("LegacyFairy.UI.CantCast".Translate((int)selectStuff.BaseMarketValue), MessageTypeDefOf.RejectInput, false);
                    }
                }
            }
        }
    }

    // TOW
    public class Pillar_Wish : PillarBase_Comp
    {
        public override void commandAction()
        {
            base.commandAction();
            Map map = parent.Map;
            // 現在発動可能な願いを入れる
            List<WishSelectDef> wishList = new List<WishSelectDef>();
            foreach (WishSelectDef sel in DefDatabase<WishSelectDef>.AllDefsListForReading)
            {
                if (sel.WishCalc.Test(map))
                {
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
            // 願いをランダムで3つ選ぶ
            List<WishSelectDef> selList = new List<WishSelectDef>();
            for (int i = 0; i < 3; i++)
            {
                WishSelectDef sel = wishList.RandomElement();
                wishList.RemoveAll(x => x.defName == sel.defName);
                selList.Add(sel);
            }
            // UI出現
            Dialog_RandomWish dialog = new Dialog_RandomWish();
            dialog.SetData(selList, map);
            Find.WindowStack.Add(dialog);
        }
    }

    public class Dialog_RandomWish : Window
    {
        public override Vector2 InitialSize => new Vector2(800f, 300f);

        List<WishSelectDef> wishList;

        Map map;

        string SelectDefName = "";

        public Dialog_RandomWish()
        {
            forcePause = true;
            doCloseX = false;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnClickedOutside = false;
        }

        public void SetData(List<WishSelectDef> list, Map map)
        {
            Messages.Message("LegacyFairy.UI.Wish".Translate(), MessageTypeDefOf.PositiveEvent, historical: false);
            wishList = list;
            this.map = map;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 80f);

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, inRect.height);

            Text.Font = GameFont.Small;

            listingStandard.Begin(viewRect);

            foreach (WishSelectDef thing in wishList)
            {
                listingStandard.Gap(4f);
                GUI.color = Color.white;
                if (listingStandard.RadioButton(thing.label, thing.defName == SelectDefName))
                {
                    SelectDefName = thing.defName;
                }
            }

            GUI.color = Color.white;

            listingStandard.End();

            if (!Widgets.ButtonText(new Rect(5f, inRect.height - 35f, inRect.width - 10f, 35f), "Wish!"))
            {
                return;
            }

            if (SelectDefName == "")
            {
                Messages.Message("LegacyFairy.Item.DontWish".Translate(), MessageTypeDefOf.RejectInput, historical: false);
            }
            else
            {
                //願いを叶える
                WishSelectDef start = DefDatabase<WishSelectDef>.GetNamed(SelectDefName);
                start.WishCalc.Run(map, start);

                Find.WindowStack.TryRemove(this);
            }
        }
    }

}
