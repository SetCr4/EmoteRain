using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;

namespace EmoteRain
{
    internal class Right : BSMLResourceViewController
    {
        public override string ResourceName => string.Join(".", new string[]
                {
                    base.GetType().Namespace,
					"BSP.Settings",
					base.GetType().Name,
					"bsml"
                });

		private void OnSettingChanged(object p_Value)
		{
			/// Update config
			Settings.subRain = m_SubRain.Value;
			Settings.subrainEmotecount = (int)m_SREmoteCount.slider.value;
		}

		protected override void DidActivate(bool p_FirstActivation, bool p_AddedToHierarchy, bool p_ScreenSystemEnabling)
		{
			base.DidActivate(p_FirstActivation, p_AddedToHierarchy, p_ScreenSystemEnabling);
			if (p_FirstActivation)
			{
				var l_Event = new BSMLAction(this, this.GetType().GetMethod(nameof(OnSettingChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

				/// Set values
				m_SubRain.Value = Settings.subRain;
				m_SREmoteCount.slider.value = Settings.subrainEmotecount;

				/// Bind events
				m_SubRain.onChange = l_Event;
				m_SREmoteCount.onChange = l_Event;
			}
		}

		[UIAction("test-subrain")]
		private void TestSubRainClickAction() => RequestCoordinator.subRain();

		[UIComponent("sub-rain")]
		public ToggleSetting m_SubRain;

		[UIComponent("subrain-emotecount")]
		public SliderSetting m_SREmoteCount;
	}
}