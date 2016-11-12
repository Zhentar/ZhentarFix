using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ZhentarFix
{
	class ScarringFix : Hediff_Injury
	{
		[DetourClassMethod(typeof(Hediff_Injury))]
		public void DirectHeal(float amount) 
		{
			if (amount <= 0f)
			{
				return;
			}
			if (this.FullyHealableOnlyByTend() && this.Severity - amount <= 2f)
			{
				amount = this.Severity - 2f;
			}
			if (amount <= 0f)
			{
				return;
			}
			this.Severity -= amount;
			if (this.comps != null)
			{
				foreach (HediffComp hediff in this.comps)
				{
					hediff.CompPostDirectHeal(amount);
				}
			}
		}
	}
}
