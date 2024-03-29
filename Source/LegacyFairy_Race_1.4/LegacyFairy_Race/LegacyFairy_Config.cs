﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace LegacyFairy_Race
{
	public class LegacyFairy_Settings : Mod
	{
		public LegacyFairy_Settings(ModContentPack content) : base(content)
		{
			GetSettings<LegacyFairy_Config>();
		}

		public override string SettingsCategory()
		{
			return "LegacyFairy.Config.Title".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			LegacyFairy_Config.DoWindowContents(inRect);
		}
	}

	public class LegacyFairy_Config : ModSettings
	{
		// バージョン
		public static int ver = 221;

		// アップデートバージョン
		public static int Updatever = 0;

		public override void ExposeData()
		{
			Scribe_Values.Look(ref Updatever, "Updatever", 0);
		}

		private static Vector2 scrollPosition;

		public static void DoWindowContents(Rect inRect)
		{
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + 700f);
			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 80f);

			// 通常設定
			Listing_Standard listingStandard = new Listing_Standard();
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 1.1f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(50f);
			listingStandard.GapLine();
			if (listingStandard.ButtonText("LegacyFairy.Config.Update".Translate()))
			{
				Updatever = ver;
				Dialog_Update dialog = new Dialog_Update();
				Find.WindowStack.Add(dialog);
			}
			listingStandard.End();
			Widgets.EndScrollView();
		}

	}
}
