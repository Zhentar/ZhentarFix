using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ZhentarFix
{
	class SmeltingFix
	{
		public bool Matches(Thing t)
		{
			return t.def.thingCategories.Intersect(ThingCategoryDefOf.Weapons.ThisAndChildCategoryDefs).Any() && !IsSmeltable(t);
		}

		public bool AlwaysMatches(ThingDef def)
		{
			return def.thingCategories.Intersect(ThingCategoryDefOf.Weapons.ThisAndChildCategoryDefs).Any() && !def.MadeFromStuff && def.smeltProducts == null && !CostsSmeltable(def);
		}

		private bool CostsSmeltable(ThingDef def)
		{
			return (def.costList?.ElementAtOrDefault(0)?.thingDef.stuffProps?.smeltable).GetValueOrDefault();
		}

		private bool IsSmeltable(Thing t)
		{

			return !t.def.smeltProducts.NullOrEmpty() || (t.def.MadeFromStuff && t.def.stuffProps.smeltable) || CostsSmeltable(t.def);
		}
	}
}
