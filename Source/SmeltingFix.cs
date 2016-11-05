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
			return t.def.thingCategories.Intersect(ThingCategoryDefOf.Weapons.ThisAndChildCategoryDefs).Any() && !t.Smeltable;
		}

		public bool AlwaysMatches(ThingDef def)
		{
			return def.thingCategories.Intersect(ThingCategoryDefOf.Weapons.ThisAndChildCategoryDefs).Any() && !def.MadeFromStuff && def.smeltProducts == null;
		}
	}
}
