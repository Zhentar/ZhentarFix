using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace ZhentarFix
{
	[StaticConstructorOnStartup]
	internal static class DetourInjector
	{
		private static bool DoInject()
		{
			if (!DoDetour(typeof(JobGiver_Haul), typeof(AnimalHaulFix), "TryGiveJob")) return false;

			if (!DoDetour(typeof(Hediff_Injury), typeof(ScarringFix), "DirectHeal")) return false;

			if (!DoDetour(typeof(SpecialThingFilterWorker_NonSmeltableWeapon), typeof(SmeltingFix), "Matches")) return false;
			if (!DoDetour(typeof(SpecialThingFilterWorker_NonSmeltableWeapon), typeof(SmeltingFix), "AlwaysMatches")) return false;

			return true;
		}

		#region guts
		private static Assembly Assembly => Assembly.GetAssembly(typeof(DetourInjector));

		private static string AssemblyName => Assembly.FullName.Split(',').First();

		static DetourInjector()
		{
			LongEventHandler.QueueLongEvent(Inject, "Initializing", true, null);
		}

		private static void Inject()
		{
			if (DoInject())
				Log.Message(AssemblyName + " injected.");
			else
				Log.Error(AssemblyName + " failed to get injected properly.");
		}

		private const BindingFlags UniversalBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static bool DoDetour(Type rimworld, Type mod, string method)
		{
			MethodInfo RimWorld_A = rimworld.GetMethod(method, UniversalBindingFlags);
			MethodInfo ModTest_A = mod.GetMethod(method, UniversalBindingFlags);
			if (!Detours.TryDetourFromTo(RimWorld_A, ModTest_A))
				return false;
			return true;
		}
		#endregion
	}
}
