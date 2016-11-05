﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace ZhentarFix
{
	public class AnimalHaulFix
	{
		protected Job TryGiveJob(Pawn pawn)
		{
			Predicate<Thing> validator = t => !t.IsForbidden(pawn) && HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t) && IsPlaceToPutThing(pawn, t);
			Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, ListerHaulables.ThingsPotentiallyNeedingHauling(), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator);
			if (thing != null)
			{
				return HaulAIUtility.HaulToStorageJob(pawn, thing);
			}
			return null;
		}

		private bool IsPlaceToPutThing(Pawn p, Thing t)
		{
			StoragePriority currentPriority = HaulAIUtility.StoragePriorityAtFor(t.Position, t);
			IntVec3 storeCell;
			if (!StoreUtility.TryFindBestBetterStoreCellFor(t, p, currentPriority, p.Faction, out storeCell))
			{
				return false;
			}
			return true;
		}

	}
}
