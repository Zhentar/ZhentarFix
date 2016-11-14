using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ZhentarFix
{
	class Designators
	{
		[DetourClassMethod(typeof(Designator_RemoveFloor))]
		public AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds() || c.Fogged())
			{
				return false;
			}
			if (Find.DesignationManager.DesignationAt(c, DesignationDefOf.RemoveFloor) != null)
			{
				return false;
			}
			Building edifice = c.GetEdifice();
			if (edifice != null && edifice.def.Fillage == FillCategory.Full && edifice.def.passability == Traversability.Impassable)
			{
				return false;
			}
			if (!Find.TerrainGrid.TerrainAt(c).layerable)
			{
				return "TerrainMustBeRemovable".Translate();
			}
			return AcceptanceReport.WasAccepted;
		}
	}
}
