using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;

namespace ZhentarFix.Source
{
	class StoryWatcher_RampUpFix : StoryWatcher_RampUp
	{
		private static readonly Func<StoryWatcher_RampUp, float> shortTermFactorGet = Utils.GetFieldAccessor<StoryWatcher_RampUp, float>("shortTermFactor");

		private static readonly FieldInfo shortTermFactorInfo = typeof(StoryWatcher_RampUp).GetField("shortTermFactor", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

		private float shortTermFactor { get { return shortTermFactorGet(this); } set { shortTermFactorInfo.SetValue(this, value); } }

		private static readonly FieldInfo longTermFactorInfo = typeof(StoryWatcher_RampUp).GetField("longTermFactor", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

		private static readonly Func<StoryWatcher_RampUp, float> longTermFactorGet = Utils.GetFieldAccessor<StoryWatcher_RampUp, float>("longTermFactor");

		private float longTermFactor { get { return longTermFactorGet(this); } set { longTermFactorInfo.SetValue(this, value); } }

		[DetourClassMethod(typeof(StoryWatcher_RampUp))]
		public void Notify_ColonistIncappedOrKilled(Pawn p)
		{
			if(!p.RaceProps.Humanlike) { return; }
			float num = this.shortTermFactor - 1f;
			float num2 = this.longTermFactor - 1f;
			switch (Find.MapPawns.FreeColonistsCount)
			{
				case 0:
					num *= 0f;
					num2 *= 0f;
					break;
				case 1:
					num *= 0f;
					num2 *= 0f;
					break;
				case 2:
					num *= 0f;
					num2 *= 0f;
					break;
				case 3:
					num *= 0f;
					num2 *= 0.2f;
					break;
				case 4:
					num *= 0.15f;
					num2 *= 0.4f;
					break;
				case 5:
					num *= 0.25f;
					num2 *= 0.6f;
					break;
				case 6:
					num *= 0.3f;
					num2 *= 0.7f;
					break;
				case 7:
					num *= 0.35f;
					num2 *= 0.75f;
					break;
				case 8:
					num *= 0.4f;
					num2 *= 0.8f;
					break;
				case 9:
					num *= 0.45f;
					num2 *= 0.85f;
					break;
				default:
					num *= 0.5f;
					num2 *= 0.9f;
					break;
			}
			this.shortTermFactor = 1f + num;
			this.longTermFactor = 1f + num2;
		}
	}
}
