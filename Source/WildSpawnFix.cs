using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ZhentarFix.Source
{
	static class WildSpawnFix
	{
		[DetourClassProperty(typeof(WildSpawner))]
		private static float CurrentTotalAnimalWeight
		{
			get
			{
				float num = 0f;
				List<Pawn> allPawnsSpawned = Find.MapPawns.AllPawnsSpawned;
				foreach (Pawn pawn in allPawnsSpawned)
				{
					if (pawn.Faction == null && pawn.kindDef.wildSpawn_spawnWild)
					{
						num += pawn.kindDef.wildSpawn_EcoSystemWeight;
					}
				}
				return num;
			}
		}

	}
}
