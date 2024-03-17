using System.Threading;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using BEPRace_Core;
using Verse.AI.Group;
using Verse.AI;

namespace LegacyFairy_Race
{
	// 召喚物
	[StaticConstructorOnStartup]
	public class TOW_SummonThing : ThingWithComps
	{
		private static readonly Material material = MaterialPool.MatFrom("LegacyFairy/UI/TOW/Fall", ShaderDatabase.Cutout, Color.white);

		private int lifeTime = 600;

		private const int maxlife = 600;

		private int phase = 0;

		public override void Tick()
		{
			base.Tick();
			lifeTime--;
			if (phase == 0) {
				if (lifeTime == 600) {
					Sound_LegacyFairy.LgcF_Fall.PlayOneShot(this);
				}
				if (lifeTime == 180) {
					GenExplosion.DoExplosion(this.Position, this.Map, 5.9f, DamageDefOf.Bomb, this, 10, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 150) {
					GenExplosion.DoExplosion(this.Position, this.Map, 5.9f, DamageDefOf.Bomb, this, 10, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 120) {
					GenExplosion.DoExplosion(this.Position, this.Map, 5.9f, DamageDefOf.Bomb, this, 10, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 90) {
					GenExplosion.DoExplosion(this.Position, this.Map, 5.9f, DamageDefOf.Bomb, this, 10, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 60) {
					GenExplosion.DoExplosion(this.Position, this.Map, 5.9f, DamageDefOf.Bomb, this, 10, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime <= 0) {
					phase = 1;
					lifeTime = 180;
					return;
				}
			}
			if (phase == 1) {
				if (lifeTime == 179) {
                    ScreenFader.StartFade(Color.white, 1.5f);
					GenExplosion.DoExplosion(this.Position, this.Map, 5.9f, DamageDefOf.Frostbite, this, 1000, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 160) {
					GenExplosion.DoExplosion(this.Position, this.Map, 10.9f, DamageDefOf.Frostbite, this, 1000, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 140) {
					GenExplosion.DoExplosion(this.Position, this.Map, 15.9f, DamageDefOf.Frostbite, this, 1000, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 120) {
					GenExplosion.DoExplosion(this.Position, this.Map, 20.9f, DamageDefOf.Frostbite, this, 1000, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime == 100) {
					GenExplosion.DoExplosion(this.Position, this.Map, 30.9f, DamageDefOf.Frostbite, this, 1000, 5.0f, SoundDef.Named("Explosion_Bomb"), damageFalloff: true);
				}
				if (lifeTime <= 0) {

					List<Pawn> Summons = new List<Pawn>();

					// TOW召喚
					PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("LgcF_TOW"));
                	Pawn SummonPawn = PawnGenerator.GeneratePawn(request);
                	SummonPawn.SetFactionDirect(Faction.OfMechanoids);
                	GenSpawn.Spawn(SummonPawn, this.Position, this.Map);

					Summons.Add(SummonPawn);

                	LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_AssaultColony(), Map, Summons);

					// フェード
                    ScreenFader.StartFade(Color.clear, 1.0f);
					this.Destroy();
				}
			}
		}
		public override void Draw()
		{
			if (phase == 0 && lifeTime > 0)
			{
				Vector3 pos = this.Position.ToVector3() + Vector3.forward * Mathf.Lerp(60f, 0f, 1.0f - (float)((float)lifeTime / maxlife));
				pos.z += 1.25f;
				pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(pos, Quaternion.Euler(0f, 180f, 0f), new Vector3(2.5f, 1f, 2.5f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lifeTime, "lifeTime", 0);
			Scribe_Values.Look(ref phase, "phase", 0);
		}
	}

	// 範囲爆発
	[StaticConstructorOnStartup]
	public class TOW_Debilitated : ThingWithComps
	{
		private static readonly Material material_A = MaterialPool.MatFrom("LegacyFairy/UI/TOW/WishBall", ShaderDatabase.Cutout, Color.white);

		private int lifeTime = 300;

		private const int maxlife = 300;

		public override void Tick()
		{
			base.Tick();
			lifeTime--;
			if (lifeTime <= 0) {
				// 爆発
				if (Map == null)
                {
					Destroy();
					return;
				}
				// エフェクト召喚
				Effect_LegacyFairy.LgcF_DebilitatedWish.Spawn(Position, Map, Vector3.zero);
				// 範囲内爆発
				List<Pawn> pawn = Map.mapPawns.AllPawnsSpawned.Where(x => x.Position.DistanceTo(this.Position) <= 5.9f && x.def.defName != "LgcF_TOW").ToList();
				for (int i = pawn.Count - 1; i >= 0; i--)
            	{
					pawn[i].TakeDamage(new DamageInfo(DamageDefOf.Frostbite, 50f, 999f, -1f));
					if (!pawn[i].Dead) {
						// 死んでいない場合デバフを付与
						pawn[i].health.AddHediff(HediffDef.Named("LgcF_Debilitated"));
					}
				}
				GenExplosion.DoExplosion(Position, Map, 5.9f, DamageDefOf.Frostbite, null, 50, 2.0f, SoundDefOf.Execute_Cut, damageFalloff: false, screenShakeFactor: 0, doVisualEffects: false);
				this.Destroy();
			}
		}
		public override void Draw()
		{
			if (lifeTime > 0)
			{
				base.Draw();
				Vector3 pos = this.Position.ToVector3() + Vector3.forward * Mathf.Lerp(60f, 0f, 1.0f - (float)((float)lifeTime / maxlife));
				pos.z += 1.25f;
				pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(pos, Quaternion.Euler(0f, 180f, 0f), new Vector3(2.5f, 1f, 2.5f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, material_A, 0);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lifeTime, "lifeTime", 0);
		}
	}

	// TOW本体処理
	public class CompProperties_TOW : CompProperties
	{
		public CompProperties_TOW()
		{
			compClass = typeof(Comp_TOW);
		}
	}

	public class Comp_TOW : ThingComp
	{
		int phase = 0;

		int nexttime = 600;

		int charge = 0;

		public override void CompTick()
		{
			nexttime--;
			if (nexttime < 0) {
				Pawn pawn = (Pawn)this.parent;
				Effecter_BEPCore.BEP_UseSkill_E.Spawn(pawn.Position, pawn.Map, Vector3.zero);
				if (phase == 0) {
					// 周囲にレガシーフェアリー3匹召喚
					// 破壊不可能タレット
					CellRect cellRect = CellRect.CenteredOn(pawn.Position, 20);
                    cellRect.ClipInsideMap(pawn.Map);
					List<Pawn> summons = new List<Pawn>();
                    for (int i = 0; i < 3; i++)
                    {
                        IntVec3 randomCell = cellRect.RandomCell;
                        Effecter_BEPCore.BEP_UseSkill_E.Spawn(randomCell, pawn.Map);
						PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("LegacyFairy_Fighter"));
						Pawn SummonPawn = PawnGenerator.GeneratePawn(request);
						SummonPawn.SetFactionDirect(Faction.OfMechanoids);
						summons.Add((Pawn)GenSpawn.Spawn(SummonPawn, randomCell, pawn.Map));
                    }
					LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_DefendPoint(summons.First().Position, 30.9f), this.parent.Map, summons);
					nexttime = 1200;
				}
				if (phase == 1) {
					// シールドタレット召喚
					CellRect cellRect = CellRect.CenteredOn(pawn.Position, 20);
                    cellRect.ClipInsideMap(pawn.Map);
                    for (int i = 0; i < 3; i++)
                    {
                        IntVec3 randomCell = cellRect.RandomCell;
                        Effecter_BEPCore.BEP_UseSkill_E.Spawn(randomCell, pawn.Map);
                        TOW_Shield shot = (TOW_Shield)GenSpawn.Spawn(ThingDef.Named("LgcF_TOW_Shield"), randomCell, pawn.Map);
						shot.owner = pawn;
                    }
					nexttime = 1200;
				}
				if (phase == 2) {
					// 破壊不可能タレット
					CellRect cellRect = CellRect.CenteredOn(pawn.Position, 20);
                    cellRect.ClipInsideMap(pawn.Map);
                    for (int i = 0; i < 15; i++)
                    {
                        IntVec3 randomCell = cellRect.RandomCell;
                        Effecter_BEPCore.BEP_UseSkill_E.Spawn(randomCell, pawn.Map);
                        Thing shot = GenSpawn.Spawn(ThingDef.Named("LgcF_TOW_Shot"), randomCell, pawn.Map);
						shot.TryGetComp<Comp_Shot>().owner = pawn;
                    }
					nexttime = 600;
				}
				if (phase == 3) {
					// 隕石召喚詠唱 + 自身スタン + 弱体化
					Messages.Message("LegacyFairy.TOW.MeteorCharge".Translate(), new LookTargets(pawn), MessageTypeDefOf.NegativeEvent);
                    pawn.stances.SetStance(new Stance_Cooldown(1300, null, null));
					pawn.health.AddHediff(HediffDef.Named("LgcF_MeteorCharge"));
					nexttime = 600;
					charge = 100;
				}
				if (phase == 4) {
					// 隕石召喚警告
					Messages.Message("LegacyFairy.TOW.MeteorChargeWarn".Translate(), new LookTargets(pawn), MessageTypeDefOf.NegativeEvent);
					nexttime = 600;
				}
				if (phase == 5) {
					if (charge > 0) {
						GenSpawn.Spawn(ThingDef.Named("LgcF_TOW_DeathMeteor"), pawn.Position, pawn.Map);
					} else {
						// 視界内の全員に爆弾
						List<Pawn> pawns = this.parent.Map.mapPawns.AllPawnsSpawned.Where(x => x.HostileTo(this.parent) && x.CanSee(this.parent)).ToList();
						if (!pawns.NullOrEmpty()) {
							foreach (Pawn pawn_tag in pawns) {
								Effecter_BEPCore.BEP_UseSkill_E.Spawn(pawn_tag.Position, pawn_tag.Map);
								GenSpawn.Spawn(Thing_LegacyFairy.LgcF_TOW_DebilitatedWish, pawn_tag.Position, pawn_tag.Map);
							}
						}
					}
					nexttime = 1200;
					charge = 0;
				}
				phase++;
				if (phase > 5) {
					phase = 0;
				}
			}
		}

		public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if (absorbed == false)
            {
                if (dinfo.Def == DamageDefOf.EMP || dinfo.Def == DamageDefOf.Stun)
                {
                    absorbed = true;
                    return;
                }
                if (this.parent.TryGetComp<Comp_BossHP>().GetHP() > 0)
                {
					if (Rand.Range(0, 20) < 1) {
						List<Pawn> pawns = this.parent.Map.mapPawns.AllPawnsSpawned.Where(x => x.HostileTo(this.parent) && x.CanSee(this.parent)).ToList();
						if (!pawns.NullOrEmpty()) {
							GenSpawn.Spawn(Thing_LegacyFairy.LgcF_TOW_DebilitatedWish, pawns.RandomElement().Position, this.parent.Map);
						}
					}
                    if (dinfo.Amount > 20.0f)
                    {
                        dinfo.SetAmount(20.0f);
                    }
                    Pawn pawn = (Pawn)this.parent;
                    int DamageAmount = Math.Max((int)(dinfo.Amount * pawn.GetStatValue(StatDefOf.IncomingDamageFactor)), 1);
					DamageAmount = Math.Min(DamageAmount, 20);
                    Effecter_BEPCore.BEP_Parry_B.Spawn(pawn.Position, pawn.Map, Vector3.zero);
                    FPDamage(DamageAmount);
                    absorbed = true;
                }
            }
        }

        public void FPDamage(float value)
        {
            Pawn pawn = (Pawn)this.parent;
            this.parent.TryGetComp<Comp_BossHP>().HPDamage((int)value);
			if (charge > 0) {
				charge--;
				if (charge == 0) {
					Effecter_BEPCore.BEP_Parry_Break.Spawn(pawn.Position, pawn.Map, Vector3.zero);
					Messages.Message("LegacyFairy.TOW.MeteorCancel".Translate(), new LookTargets(pawn), MessageTypeDefOf.PositiveEvent);
				}
			}
            if (this.parent.TryGetComp<Comp_BossHP>().GetHP() > 0)
            {
                return;
            }
            Effecter_BEPCore.BEP_Parry_Break.Spawn(pawn.Position, pawn.Map, Vector3.zero);
            if (pawn.Spawned)
            {
                if (pawn.Map != null)
                {
                    Effecter_BEPCore.BEP_UseSkill_D.Spawn(pawn.Position, pawn.Map, Vector3.zero);
                    if (pawn.Faction?.IsPlayer ?? false || pawn.Faction == null)
                    {
                        pawn.Destroy();
                        return;
                    }
                    // アイテムをドロップする
                    Thing thing = ThingMaker.MakeThing(ThingDef.Named("LgcF_RodOfAwakening"));
                    GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                }
            }
            pawn.Destroy();
            return;
        }

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref phase, "phase", 0);
			Scribe_Values.Look(ref nexttime, "nexttime", 0);
			Scribe_Values.Look(ref charge, "charge", 0);
		}
    }

	public class CompProperties_Shot : CompProperties
	{
		public CompProperties_Shot()
		{
			this.compClass = typeof(Comp_Shot);
		}
	}

	public class Comp_Shot : ThingComp
	{
		// 消滅までのTick
		public int tick = 1200;
		// ギアの回転
		public bool roll = false;
		// 使用者
		public Pawn owner;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tick, "tick", 1200);
			Scribe_References.Look(ref owner, "owner");
		}

		public override void CompTick()
		{
			base.CompTick();
			if (this.parent.IsHashIntervalTick(3))
			{
				roll = !roll;
			}
			tick--;
			if (this.parent.IsHashIntervalTick(180))
            {
				Pawn tagpawn = this.parent.Map.mapPawns.AllPawnsSpawned.Where(x => x.HostileTo(owner) && this.parent.Position.DistanceTo(x.Position) < 20.9f && this.parent.CanSee(x) && !x.Downed).RandomElement();
				if (tagpawn != null)
                {
					Projectile shot = (Projectile)GenSpawn.Spawn(ThingDef.Named("LgcF_Cluster"), this.parent.Position, this.parent.Map);
					IntVec3 c = tagpawn.Position;
					Vector3 drawPos = this.parent.DrawPos;
					shot.Launch(this.parent, drawPos, c, tagpawn.Position, ProjectileHitFlags.All, false, null);
				}
			}
			if (tick <= 0)
			{
				this.parent.Destroy();
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			Vector3 pos = parent.TrueCenter();
			pos.y = AltitudeLayer.Floor.AltitudeFor();
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(pos, Quaternion.identity, new Vector3(GetSize() * 1.16015625f, 1f, GetSize() * 1.16015625f));
			if (roll)
			{
				Graphics.DrawMesh(MeshPool.plane10, matrix, LegacyFairy_Graphic.Mat_ShotGear_A, 0, null, 0, LegacyFairy_Graphic.MatPropertyBlock);
			}
			else
			{
				Graphics.DrawMesh(MeshPool.plane10, matrix, LegacyFairy_Graphic.Mat_ShotGear_B, 0, null, 0, LegacyFairy_Graphic.MatPropertyBlock);
			}
		}

		private float GetSize()
		{
			if (tick < 10)
			{
				return Math.Min(0.1f * tick, 1.0f);
			}
			if (tick > 0)
			{
				return Math.Min(0.1f * (1200 - tick), 1.0f);
			}
			return 0.0f;
		}
	}

	// ギミック失敗時のメテオ
	public class TOW_DeathMeteor : ThingWithComps
	{
		private int lifeTime = 180;

		public override void Tick()
		{
			base.Tick();
			lifeTime--;
			if (lifeTime == 179) {
                ScreenFader.StartFade(Color.white, 1.5f);
				SoundDef.Named("LgcF_DeathMeteor").PlayOneShot(this);
			}
			if (lifeTime == 160) {
				AllDamage();
			}
			if (lifeTime == 140) {
				AllDamage();
			}
			if (lifeTime == 120) {
				AllDamage();
			}
			if (lifeTime == 100) {
				AllDamage();
			}
			if (lifeTime == 80) {
				AllDamage();
			}
			if (lifeTime == 60) {
				AllDamage();
			}
			if (lifeTime == 40) {
				AllDamage();
			}
			if (lifeTime == 20) {
				AllDamage();
			}
			if (lifeTime <= 0) {
				List<Pawn> Summons = new List<Pawn>();
				// フェード
                ScreenFader.StartFade(Color.clear, 1.0f);
				this.Destroy();
			}
		}

		public void AllDamage() {
			List<Pawn> pawns = this.Map.mapPawns.AllPawnsSpawned.Where(x => x.def.defName != "LgcF_TOW").ToList();
            for (int i = pawns.Count() - 1; i >= 0; i--)
            {
                Pawn selPawn = pawns[i];
                selPawn.TakeDamage(new DamageInfo(RimWorld.DamageDefOf.Frostbite, 2000.0f, 999.0f, -1, selPawn, null, null));
                selPawn.TakeDamage(new DamageInfo(RimWorld.DamageDefOf.Blunt, 2000.0f, 999.0f, -1, selPawn, null, null));
            }
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lifeTime, "lifeTime", 0);
		}
	}

	// シールド
	public class TOW_Shield : ThingWithComps
	{
		private int lifeTime = 3600;

		public Pawn owner;

		public override void Tick()
		{
			base.Tick();
			if (lifeTime <= 0) {
				Destroy();
				return;
			}
			lifeTime--;
			if (this.IsHashIntervalTick(3)) {
				IEnumerable<Thing> Projs = Map.listerThings.AllThings.Where(x => x as Projectile != null & x.Position.DistanceTo(Position) <= 5.9f);
				if (!Projs.EnumerableNullOrEmpty())
				{
					for (int i = Projs.Count() - 1; i >= 0; i--)
					{
						Projectile proj = (Projectile)Projs.ElementAt(i);
						if (proj.Launcher?.Spawned ?? false)
						{
							if (proj.Launcher?.HostileTo(owner) ?? false)
							{
								Effecter_BEPCore.BEP_Parry_B.Spawn(proj.Position, proj.Map, Vector3.zero);
								for (int i2 = 0; i2 < 6; i2++)
								{
									FleckMaker.ThrowDustPuff(proj.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), proj.Map, Rand.Range(0.8f, 1.2f));
								}
								proj.Destroy();
							}
						}
					}
				}
			}
		}

		public override void Draw()
		{
			base.Draw();
			Vector3 pos = this.TrueCenter();
            pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            float currentAlpha = 0.5f;
            if (currentAlpha > 0f)
            {
                Matrix4x4 matrix = default;
                matrix.SetTRS(pos, Quaternion.identity, new Vector3(5.9f * 2f * 1.16015625f, 1f, 5.9f * 2f * 1.16015625f));
                Graphics.DrawMesh(MeshPool.plane10, matrix, LegacyFairy_Graphic.Shield, 0, null, 0, LegacyFairy_Graphic.ShieldBlock);
            }
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lifeTime, "lifeTime", 0);
			Scribe_References.Look(ref owner, "owner");
		}
	}

}