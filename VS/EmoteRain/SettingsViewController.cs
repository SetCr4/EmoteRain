using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;

namespace EmoteRain
{
    internal class SettingsViewController : PersistentSingleton<SettingsViewController>
    {
        //public override string ResourceName => string.Join(".", GetType().Namespace, "Views.settings.bsml");

        [UIValue("menu-rain")]
        private bool menuRain = true;

        [UIValue("size-in-menu")]
        private float menuSize = 0.4f;

        [UIValue("song-rain")]
        private bool songRain = true;

        [UIValue("size-in-song")]
        private float songSize = 0.6f;
    }
}
