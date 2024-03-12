using BEPRace_Core;
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
    public class Gizmo_Protection : Gizmo
    {
        public HediffComp_Protection comp;

        private static readonly Texture2D FullNPBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f));

        private static readonly Texture2D EmptyNPBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public Gizmo_Protection()
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
            Widgets.Label(rect3, "LegacyFairy.UI.Protect".Translate());
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = comp.parent.Severity / Mathf.Max(1f, 90);
            Widgets.FillableBar(rect4, fillPercent, FullNPBarTex, EmptyNPBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, comp.parent.Severity.ToString() + "/ 90");
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }
    }

    public class HediffComp_Protection : HediffComp
    {
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if (dinfo.Def == DamageDefOf.EMP || dinfo.Def == DamageDefOf.Stun)
            {
                return;
            }
            this.parent.Severity = Math.Max(0.0f, this.parent.Severity - Math.Max((int)dinfo.Amount, 1));

            if (this.parent.Severity <= 0.0f)
            {
                MoteMaker.ThrowText(Pawn.TrueCenter(), Pawn.Map, "Break!");
                Effecter_BEPCore.BEP_Parry_Break.Spawn(Pawn.Position, Pawn.Map, Vector3.zero);
            }
            else
            {
                MoteMaker.ThrowText(Pawn.TrueCenter(), Pawn.Map, "Protect!");
                Effecter_BEPCore.BEP_Parry_A.Spawn(Pawn.Position, Pawn.Map, Vector3.zero);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            Pawn comppawn = (Pawn)this.parent.pawn;
            if (Find.Selector.SelectedPawns.Contains(comppawn))
            {
                Gizmo_Protection gizmo = new Gizmo_Protection();
                gizmo.comp = this;
                yield return gizmo;
            }
        }

    }
}
