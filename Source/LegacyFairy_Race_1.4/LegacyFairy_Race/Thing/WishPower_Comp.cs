using BEPRace_Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using static HarmonyLib.Code;

namespace LegacyFairy_Race
{

    [StaticConstructorOnStartup]
    public class Gizmo_WishPowerStatus : Gizmo
    {
        public Comp_WishPower comp;

        private static readonly Texture2D FullNPBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f));

        private static readonly Texture2D EmptyNPBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public Gizmo_WishPowerStatus()
        {
            Order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, "LegacyFairy.UI.WP".Translate());
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = comp.nowPower / Mathf.Max(1f, comp.Props.maxPower);
            Widgets.FillableBar(rect4, fillPercent, FullNPBarTex, EmptyNPBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, comp.nowPower.ToString() + "/" + comp.Props.maxPower.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }

    }

    [StaticConstructorOnStartup]
    public class Gizmo_WishPowerStatusBad : Gizmo
    {
        public Comp_WishPower comp;

        private static readonly Texture2D FullNPBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0.2f, 0.2f));

        private static readonly Texture2D EmptyNPBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public Gizmo_WishPowerStatusBad()
        {
            Order = -99f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            int badcount = Find.World.GetComponent<LegacyFairy_WorldComponent>().UsedWP;
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, "LegacyFairy.UI.WPBad".Translate());
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = badcount / 100f;
            Widgets.FillableBar(rect4, fillPercent, FullNPBarTex, EmptyNPBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, badcount.ToString() + "/" + "100");
            Text.Anchor = TextAnchor.UpperLeft;
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                string tip = "LegacyFairy.UI.WPBadDesc".Translate();
                if (!tip.NullOrEmpty())
                {
                    TooltipHandler.TipRegion(rect, () => tip, Gen.HashCombineInt(comp.GetHashCode(), 7477453));
                }
            }
            return new GizmoResult(GizmoState.Clear);
        }

    }

    /// <summary>
    /// レガシーフェアリーからWPを受け取ったり与えたりする所
    /// </summary>
    public class CompProperties_WishPower : CompProperties
    {
        // 貯蓄機か？
        public bool isSaving = false;

        // 最大貯蔵量
        public int maxPower = 100;

        // チャージ量
        public int drainPower = 1;

        // 消費量
        public int usePower = 1;

        public CompProperties_WishPower()
        {
            compClass = typeof(Comp_WishPower);
        }
    }

    public class Comp_WishPower : ThingComp
    {
        public int nowPower = 0;

        public CompProperties_WishPower Props => (CompProperties_WishPower)props;

        public bool isMaxPower => nowPower == Props.maxPower;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nowPower, "nowPower");
        }

        public void Charge(int power)
        {
            nowPower = Math.Min(Props.maxPower, nowPower + power);
        }

        // 消費・受け渡し
        public bool Used(int power, Thing thing)
        {
            // もし受け渡しの場合、あるだけ渡す
            if (thing != null)
            {
                if (nowPower < power)
                {
                    thing.TryGetComp<Comp_WishPower>().Charge(nowPower);
                } else
                {
                    thing.TryGetComp<Comp_WishPower>().Charge(power);
                }
                nowPower = Math.Max(0, nowPower - power);
                return true;
            }
            // 消費の場合は量が超えているなら消費する
            if (nowPower >= power)
            {
                nowPower = Math.Max(0, nowPower - power);
                // 不和を増やす
                Find.World.GetComponent<LegacyFairy_WorldComponent>().AddWP(power, parent.Map);
                return true;
            }
            return false;
        }

        public override void CompTick()
        {
            // 1時間ごとに処理
            if (parent.IsHashIntervalTick(2500))
            {
                if (parent.Map == null)
                {
                    return;
                }
                if (Props.isSaving)
                {
                    // 貯蓄機の場合、周囲にいるレガシーフェアリーと願いの遺伝子を持っているポーンを検索
                    List<Pawn> pawns = parent.Map.mapPawns.AllPawnsSpawned;
                    int wishcount = pawns.Where(x => x.def.defName == "LegacyFairy_Pawn").Count();
                    if (ModLister.BiotechInstalled)
                    {
                        wishcount += pawns.Where(x => x.genes != null && x.genes.HasGene(Gene_BEPCore.BEP_WishPower)).Count();
                    }
                    Charge(wishcount);
                    Effecter_BEPCore.BEP_UseSkill_H.Spawn(parent, parent.Map);

                    // 更に、周囲にある他のWP消費機械を探して分配する
                    List<Thing> things = parent.Map.spawnedThings.Where(x => x.Position.DistanceTo(parent.Position) <= parent.def.specialDisplayRadius && x.TryGetComp<Comp_WishPower>() != null && x != parent).ToList();
                    foreach (Thing thing in things)
                    {
                        if (nowPower <= 0)
                        {
                            break;
                        }
                        Comp_WishPower power = thing.TryGetComp<Comp_WishPower>();
                        if (!power.isMaxPower)
                        {
                            Used(Math.Min(power.Props.drainPower, power.Props.maxPower - power.nowPower), thing);
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Gizmo_WishPowerStatus gizmo_WPStatus = new Gizmo_WishPowerStatus();
            gizmo_WPStatus.comp = this;
            yield return gizmo_WPStatus;
            Gizmo_WishPowerStatusBad gizmo_WPStatusBad = new Gizmo_WishPowerStatusBad();
            gizmo_WPStatusBad.comp = this;
            yield return gizmo_WPStatusBad;
        }

    }
}
