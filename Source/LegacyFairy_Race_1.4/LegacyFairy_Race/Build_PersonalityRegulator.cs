using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LegacyFairy_Race
{
    public class Build_PersonalityRegulator : Building_Bed
    {
        private int braintick = 0;

        public override void Draw()
        {
            base.Draw();
            Vector3 vector = base.DrawPos;
            Vector3 s = new Vector3(2.0f, 0.5f, 2.0f);

            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(0.0f, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, this.GetCurOccupant(0) != null ? LegacyFairy_Graphic.HeadSet : LegacyFairy_Graphic.HeadSet_Open, 0);
        }

        public override void Tick()
        {
            base.Tick();
            Pawn pawn = this.GetCurOccupant(0);
            if (pawn != null)
            {
                if (ModLister.IdeologyInstalled)
                {
                    if (pawn.IsSlaveOfColony)
                    {
                        braintick = 0;
                        return;
                    }
                } else
                {
                    if (pawn.IsColonist)
                    {
                        braintick = 0;
                        return;
                    }
                }
                // 洗脳開始
                if (braintick == 0)
                {
                    pawn.health.AddHediff(HediffDef.Named("LgcF_Binding"));
                }
                if (this.def.defName == "LgcF_PersonalityRegulator")
                {
                    // 洗脳完了
                    if (braintick >= 60000)
                    {
                        if (ModLister.IdeologyInstalled)
                        {
                            try
                            {
                                pawn.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
                                Messages.Message("LegacyFairy.UI.BrainWash".Translate(pawn), new LookTargets(pawn), MessageTypeDefOf.NeutralEvent);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            pawn.SetFaction(Faction.OfPlayer);
                        }
                        braintick = 0;
                        return;
                    }
                    if (pawn.IsHashIntervalTick(60))
                    {
                        pawn.health.AddHediff(HediffDef.Named("LgcF_Binding"));
                    }
                }
                if (this.def.defName == "LgcF_ExecutionBed")
                {
                    // 処刑
                    if (braintick >= 2500)
                    {
                        int num = 3;
                        for (int i = 0; i < num; i++)
                        {
                            pawn.health.DropBloodFilth();
                        }
                        BodyPartRecord bodyPartRecord = ExecutionUtility.ExecuteCutPart(pawn);
                        int num2 = 99999;
                        if (ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.HasGene(GeneDefOf.Deathless))
                        {
                            if (pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
                            {
                                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.MechlinkImplant);
                                if (bodyPartRecord.GetPartAndAllChildParts().Contains(firstHediffOfDef.Part))
                                {
                                    GenSpawn.Spawn(ThingDefOf.Mechlink, pawn.Position, pawn.Map);
                                }
                            }
                        }
                        DamageInfo damageInfo = new DamageInfo(DamageDefOf.ExecutionCut, num2, 999f, -1f, null, bodyPartRecord, null, DamageInfo.SourceCategory.ThingOrUnknown, null, false, true);
                        pawn.TakeDamage(damageInfo);
                        if (!pawn.Dead)
                        {
                            pawn.Kill(damageInfo);
                        }
                        SoundDefOf.Execute_Cut.PlayOneShot(pawn);
                        braintick = 0;
                        return;
                    }
                    if (pawn.IsHashIntervalTick(60))
                    {
                        pawn.health.AddHediff(HediffDef.Named("LgcF_Binding"));
                    }
                }

                braintick++;
            } else
            {
                braintick = 0;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref braintick, "braintick", 0);
        }

    }

	

}
