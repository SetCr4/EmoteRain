using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;

namespace EmoteRain
{
    internal class Credits : BSMLResourceViewController
    {
        public override string ResourceName => string.Join(".", new string[]
                {
                    base.GetType().Namespace,
					"BSP.Settings",
					base.GetType().Name,
					"bsml"
				});

		protected override void DidActivate(bool p_FirstActivation, bool p_AddedToHierarchy, bool p_ScreenSystemEnabling)
		{
			base.DidActivate(p_FirstActivation, p_AddedToHierarchy, p_ScreenSystemEnabling);
			if (p_FirstActivation)
			{
				Color color = this.CreditBackground.GetComponent<ImageView>().color;
				color.a = 0.5f;
				this.CreditBackground.GetComponent<ImageView>().color = color;
			}
		}

		[UIObject("CreditBackground")]
		internal GameObject CreditBackground;

		[UIValue("Line1")]
		private readonly string m_Line1 = "<u><b>EmoteRain</b> - Let em' rain!</u>";

		[UIValue("Line2")]
		private readonly string m_Line2 = "Made by <b>Cr4</b> and <b>Uialeth</b>";

		[UIValue("Line3")]
		private readonly string m_Line3 = "Check out the README at https://github.com/SetCr4/EmoteRain";

		[UIValue("Line4")]
		private readonly string m_Line4 = " ";

		[UIValue("Line5")]
		private readonly string m_Line5 = " ";
	}
}