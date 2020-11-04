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
    internal class Combomode : BSMLResourceViewController
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
			Settings.comboMode = m_ComboMode.Value;
			Settings.comboTimer = m_ComboTimer.slider.value;
			Settings.comboCount = (int)m_ComboCount.slider.value;
		}

		protected override void DidActivate(bool p_FirstActivation, bool p_AddedToHierarchy, bool p_ScreenSystemEnabling)
		{
			base.DidActivate(p_FirstActivation, p_AddedToHierarchy, p_ScreenSystemEnabling);
			if (p_FirstActivation)
			{
				Color color = this.InfoBG2.GetComponent<ImageView>().color;
				color.a = 0.5f;
				this.InfoBG2.GetComponent<ImageView>().color = color;

				var l_Event = new BSMLAction(this, this.GetType().GetMethod(nameof(OnSettingChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

				/// Set values
				m_ComboMode.Value = Settings.comboMode;
				m_ComboTimer.slider.value = Settings.comboTimer;
				m_ComboCount.slider.value = Settings.subrainEmotecount;

				/// Bind events
				m_ComboMode.onChange = l_Event;
				m_ComboTimer.onChange = l_Event;
				m_ComboCount.onChange = l_Event;
			}
		}
		[UIObject("infobg2")]
		internal GameObject InfoBG2;

		[UIComponent("combo-mode")]
		public ToggleSetting m_ComboMode;

		[UIComponent("combo-timer")]
		public SliderSetting m_ComboTimer;

		[UIComponent("combo-count")]
		public SliderSetting m_ComboCount;
	}
}