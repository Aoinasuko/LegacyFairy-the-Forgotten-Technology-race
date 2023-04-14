using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LegacyFairy_Race
{
	[StaticConstructorOnStartup]
	public class Gizmo_WPStatus : Gizmo
	{
		public Comp_LegacyFairy comp;

		private static readonly Texture2D FullNPBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f));

		private static readonly Texture2D EmptyNPBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

		public Gizmo_WPStatus()
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
			float fillPercent = comp.NowWP / Mathf.Max(1f, comp.MaxWP);
			Widgets.FillableBar(rect4, fillPercent, FullNPBarTex, EmptyNPBarTex, doBorder: false);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect4, comp.NowWP.ToString() + "/" + comp.MaxWP.ToString());
			Text.Anchor = TextAnchor.UpperLeft;
			return new GizmoResult(GizmoState.Clear);
		}

	}

	public class CompProperties_LegacyFairy : CompProperties
	{
		public CompProperties_LegacyFairy()
		{
			compClass = typeof(Comp_LegacyFairy);
		}
	}

	public class Comp_LegacyFairy : ThingComp
	{
		// 最大WP
		public int MaxWP = 60;
		// 現在WP
		public int NowWP = 0;
		// クールダウン
		public int CoolDown = 0;
		// 最初のスポーン
		public bool FirstSpawn = false;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref MaxWP, "MaxWP");
			Scribe_Values.Look(ref NowWP, "NowWP");
			Scribe_Values.Look(ref CoolDown, "CoolDown");
			Scribe_Values.Look(ref FirstSpawn, "FirstSpawn");
		}

		// WP最大値のセット
		public void SetWPMax()
		{
			int Cal_WPMax = 0;
			Cal_WPMax += 60;
			MaxWP = Math.Max(Cal_WPMax, 1);
		}

		public int GetHealRate()
		{
			Pawn pawn = (Pawn)this.parent;
			int WPHealRate = 0;
			return Math.Max(WPHealRate + 1, 0);
		}

		public void WPHeal(int value)
        {
			NowWP += value;
			NowWP = Math.Min(NowWP, MaxWP);
		}

		public void WPUse(int value)
		{
			NowWP -= value;
			NowWP = Math.Max(NowWP, 0);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (FirstSpawn == false)
			{
				if (respawningAfterLoad == false)
				{
					NowWP = 60;
					FirstSpawn = true;
				}
			}
		}

		public override void CompTick()
		{
			Pawn pawn = (Pawn)this.parent;
			if (parent.IsHashIntervalTick(60))
            {
				if (!pawn.Faction?.IsPlayer ?? true)
                {
					if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("LgcF_BrillianceCharge")))
					{
						pawn.health.AddHediff(HediffDef.Named("LgcF_BrillianceCharge"));
					}
					return;
                }
				if (pawn.health.hediffSet.HasHediff(HediffDef.Named("LgcF_BrillianceCharge")))
				{
					if (pawn.Downed)
					{
						pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("LgcF_BrillianceCharge")));
					} else
                    {
						WPUse(1);
						if (NowWP <= 0)
                        {
							pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("LgcF_BrillianceCharge")));
						}
					}
				}
			}
			
			// 1時間ごとの処理
			if (parent.IsHashIntervalTick(2500))
			{
				// WP回復
				WPHeal(GetHealRate());
			}

			if (this.CoolDown > 0)
            {
				this.CoolDown--;
            }
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			Pawn pawn = (Pawn)this.parent;
			if ((pawn.Faction?.IsPlayer ?? false) == false)
			{
				yield break;
			}
			else
			{
				if (Find.Selector.SelectedPawns.Contains(pawn))
				{
					Gizmo_WPStatus gizmo_WPStatus = new Gizmo_WPStatus();
					gizmo_WPStatus.comp = this;
					yield return gizmo_WPStatus;
					string LowWP = "LegacyFairy.UI.disabled_LowWP".Translate();
					string Down = "LegacyFairy.UI.disabled_Down".Translate();
					string Cooldown = "LegacyFairy.UI.disabled_Cooldown".Translate();
					Gizmo Gizmo = new Command_Toggle
					{
						defaultLabel = "LegacyFairy.UI.label_BrillianceCharge".Translate(),
						icon = Graphic_LegacyFairy.BrillianceCharge_Icon,
						defaultDesc = "LegacyFairy.UI.desc_BrillianceCharge".Translate(),
						disabled = this.NowWP <= 0 || pawn.Downed,
						disabledReason = this.NowWP <= 0 ? LowWP : Down, 
						isActive = delegate
						{
							return pawn.health.hediffSet.HasHediff(HediffDef.Named("LgcF_BrillianceCharge"));
						},
						toggleAction = delegate
						{
							if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("LgcF_BrillianceCharge")))
							{
								pawn.health.AddHediff(HediffDef.Named("LgcF_BrillianceCharge"));
							}
							else
							{
								pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("LgcF_BrillianceCharge")));
							}
						}
					};
					yield return Gizmo;
					String cooldown = this.CoolDown.TicksToSeconds().ToString("F1");
					String label = CoolDown > 0 ? "LegacyFairy.UI.label_PrayertoDestiny".Translate() + "(" + cooldown + ")" : "LegacyFairy.UI.label_PrayertoDestiny".Translate();
					Gizmo Gizmo2 = new Command_Action
					{
						defaultLabel = label,
						icon = Graphic_LegacyFairy.PrayertoDestiny_Icon,
						defaultDesc = "LegacyFairy.UI.desc_PrayertoDestiny".Translate(),
						disabled = this.NowWP <= 0 || pawn.Downed || this.CoolDown > 0,
						disabledReason = this.NowWP <= 0 ? LowWP : pawn.Downed ? Down : Cooldown,
						action = delegate
                        {
							Dialog_Wish dialog = new Dialog_Wish();
							dialog.SetData(pawn);
							Find.WindowStack.Add(dialog);
							this.CoolDown = 100000;
                        }
					};
					yield return Gizmo2;
				}
			}
		}

	}

	public class Dialog_Wish : Window
	{
		public override Vector2 InitialSize => new Vector2(800f, 500f);

		IEnumerable<Pawn> deadlist;

		IEnumerable<ThingDef> thinglist;

		private static Vector2 scrollPosition;

		string SelectDefName = "";

		string Filter = "";

		int Maxval = 0;

		Pawn User;

		public Dialog_Wish()
		{
			forcePause = true;
			doCloseX = false;
			absorbInputAroundWindow = true;
			closeOnAccept = false;
			closeOnClickedOutside = false;
			deadlist = Find.World.worldPawns.AllPawnsDead.Where(x => x.Faction != null && x.Faction == Faction.OfPlayer);
		}

		public void SetData(Pawn pawn)
		{
			User = pawn;
			Maxval = pawn.TryGetComp<Comp_LegacyFairy>().NowWP * 50;
		}

		public PawnKindDef find_living(ThingDef def)
		{

			IEnumerable<PawnKindDef> pawnkinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(x => x.race == def);

			if (!pawnkinds.EnumerableNullOrEmpty())
			{
				PawnKindDef pawnkind = pawnkinds.RandomElement();
				return pawnkind;
			}
			return null;
		}

		public override void DoWindowContents(Rect inRect)
		{
			if (thinglist.EnumerableNullOrEmpty())
			{
				thinglist = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => (x.race != null && !x.race.IsMechanoid) || (x.race == null && x.thingCategories != null && x.building == null && !x.IsCorpse && x.BaseMarketValue <= Maxval));
				Messages.Message("LegacyFairy.Item.LetsWish".Translate(), MessageTypeDefOf.PositiveEvent, historical: false);
			}

			Listing_Standard listingStandard = new Listing_Standard();

			int loop = thinglist.Count();

			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 80f);

			Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, inRect.height + (loop * 24f));

			Text.Font = GameFont.Small;

			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			listingStandard.Begin(viewRect);

			string Filter_local = listingStandard.TextEntry(Filter);

			Filter = Filter_local;

			IEnumerable<ThingDef> thinglistfilter;

			if (Filter_local != "")
			{
				thinglistfilter = thinglist.Where(x => x.label.Contains(Filter_local));
			}
			else
			{
				thinglistfilter = thinglist;
			}

			foreach (ThingDef thing in thinglistfilter)
			{
				listingStandard.Gap(4f);
				if (thing.race != null)
				{
					GUI.color = Color.cyan;
					if (listingStandard.RadioButton("LegacyFairy.Item.Summon".Translate() + ": " + thing.label, thing.defName == SelectDefName))
					{
						SelectDefName = thing.defName;
					}
				}
				else
				{
					GUI.color = Color.white;
					if (listingStandard.RadioButton(thing.label, thing.defName == SelectDefName))
					{
						SelectDefName = thing.defName;
					}
				}
			}

			GUI.color = Color.white;

			listingStandard.End();
			Widgets.EndScrollView();

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
				if (ThingDef.Named(SelectDefName).race != null)
				{
					// 仲間の生成
					PawnKindDef pawnkind = find_living(ThingDef.Named(SelectDefName));
					if (pawnkind == null)
					{
						Messages.Message("LegacyFairy.Item.SoldOut".Translate(), MessageTypeDefOf.RejectInput, historical: false);
					}
					else
					{
						int usefp = (int)(ThingDef.Named(SelectDefName).BaseMarketValue / 50);
						User.TryGetComp<Comp_LegacyFairy>().WPUse(usefp);
						PawnGenerationRequest request = new PawnGenerationRequest(pawnkind);
						Pawn pawn = PawnGenerator.GeneratePawn(request);
						Map map = User.Map;
						IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
						List<Thing> things = new List<Thing>();
						things.Add(pawn);
						pawn = ThingUtility.FindPawn(things);
						if (pawn.skills != null)
						{
							for (int i = 0; i < 3; i++)
							{
								pawn.skills.skills.RandomElement().passion = Passion.Major;
							}
						}
						ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
						activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(things);
						activeDropPodInfo.openDelay = 180;
						activeDropPodInfo.leaveSlag = false;
						DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo);
						pawn.SetFaction(null);
						pawn.health.AddHediff(HediffDefOf.Anesthetic);
						Messages.Message("LegacyFairy.Item.WishEnd".Translate(pawn.Label), pawn, MessageTypeDefOf.PositiveEvent);
						Find.WindowStack.TryRemove(this);
					}
				}
				else
				{
					ThingDef thingdef = ThingDef.Named(SelectDefName);
					int count = Math.Min((int)(Maxval / thingdef.BaseMarketValue) + 1, thingdef.stackLimit);
					count = Math.Min(count, thingdef.stackLimit);
					int usefp = (int)((count * thingdef.BaseMarketValue) / 50);
					User.TryGetComp<Comp_LegacyFairy>().WPUse(usefp);
					Thing thing = ThingMaker.MakeThing(thingdef);
					thing.stackCount = count;
					Map map = User.Map;
					IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
					List<Thing> things = new List<Thing>();
					things.Add(thing);
					ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
					activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(things);
					activeDropPodInfo.openDelay = 180;
					activeDropPodInfo.leaveSlag = false;
					DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo);
					Messages.Message("LegacyFairy.Item.WishEnd".Translate(thing.Label), thing, MessageTypeDefOf.PositiveEvent);
					Find.WindowStack.TryRemove(this);
				}
			}
		}
	}
}
