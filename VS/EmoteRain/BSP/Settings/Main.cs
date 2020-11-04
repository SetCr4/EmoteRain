using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using EmoteRain.Commands;
using HMUI;
using UnityEngine;

namespace EmoteRain
{
	internal class Main : BSMLResourceViewController
	{
		public override string ResourceName => string.Join(".", new string[]
				{
					base.GetType().Namespace,
					"BSP.Settings",
					base.GetType().Name,
					"bsml"
				});

		/// <summary>
		/// When settings are changed
		/// </summary>
		/// <param name="p_Value"></param>
		private void OnSettingChanged(object p_Value)
		{
			/// Update config
			Settings.menuRain = m_MenuRain.Value;
			Settings.songRain = m_SongRain.Value;
			Settings.menuSize = m_MenuSize.slider.value;
			Settings.songSize = m_SongSize.slider.value;
			Settings.emoteDelay = (int)m_EmoteDelay.slider.value;
			Settings.emoteFallspeed = m_Fallspeed.slider.value;

		}

		protected override void DidActivate(bool p_FirstActivation, bool p_AddedToHierarchy, bool p_ScreenSystemEnabling)
		{
			base.DidActivate(p_FirstActivation, p_AddedToHierarchy, p_ScreenSystemEnabling);
			if (p_FirstActivation)
			{
				var l_Event = new BSMLAction(this, this.GetType().GetMethod(nameof(OnSettingChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

				/// Set values
				m_MenuRain.Value = Settings.menuRain;
				m_SongRain.Value = Settings.songRain;
				m_MenuSize.slider.value = Settings.menuSize;
				m_SongSize.slider.value = Settings.songSize;
				m_EmoteDelay.slider.value = Settings.emoteDelay;
				m_Fallspeed.slider.value = Settings.emoteFallspeed;

				/// Bind events
				m_MenuRain.onChange = l_Event;
				m_SongRain.onChange = l_Event;
				m_MenuSize.onChange = l_Event;
				m_SongSize.onChange = l_Event;
				m_EmoteDelay.onChange = l_Event;
				m_Fallspeed.onChange = l_Event;
			}
		}

		[UIParams]
		private BSMLParserParams m_ParserParams = null;

		[UIComponent("menu-rain")]
		public ToggleSetting m_MenuRain;

		[UIComponent("song-rain")]
		public ToggleSetting m_SongRain;

		[UIComponent("size-in-menu")]
		public SliderSetting m_MenuSize;

		[UIComponent("size-in-song")]
		public SliderSetting m_SongSize;

		[UIComponent("emote-delay")]
		public SliderSetting m_EmoteDelay;

		[UIComponent("emote-fallspeed")]
		public SliderSetting m_Fallspeed;

		[UIAction("change-subrain")]
		private void ChangeViewToSubRain() => BeatSaberPlus.UI.ViewFlowCoordinator.Instance.SetRightScreen(Plugin.sSubrain);

		[UIAction("change-combo")]
		private void ChangeViewToCombo() => BeatSaberPlus.UI.ViewFlowCoordinator.Instance.SetRightScreen(Plugin.sCombo);
	}
}