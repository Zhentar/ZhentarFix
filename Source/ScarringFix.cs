using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ZhentarFix
{
	class ScarringFix : Hediff_Injury
	{
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
				for (int i = 0; i < this.comps.Count; i++)
				{
					this.comps[i].CompPostDirectHeal(amount);
				}
			}
		}
	}
}
