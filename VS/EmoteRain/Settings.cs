using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;

namespace EmoteRain
{
    internal sealed class Settings : PersistentSingleton<Settings>
    {
        //public override string ResourceName => string.Join(".", GetType().Namespace, "Views.settings.bsml");

        [UIValue("menu-rain")]
        public static bool menuRain
        {
            get
            {
                if (_menuRain == null)
                {
                    _menuRain = Plugin.config.GetBool("Settings", "MenuRain", true, true);
                }
                return _menuRain.Value;
            }
            set
            {
                if (value != _menuRain.Value)
                {
                    Plugin.config.SetBool("Settings", "MenuRain", value);
                    _menuRain = value;
                }
            }
        }
        private static bool? _menuRain;

        [UIValue("size-in-menu")]
        public static float menuSize
        {
            get
            {
                if (_menuSize == null)
                {
                    _menuSize = Plugin.config.GetFloat("Settings", "MenuSize", 0.4f, true);
                }
                return _menuSize.Value;
            }
            set
            {
                if (value != _menuSize.Value)
                {
                    Plugin.config.SetFloat("Settings", "MenuSize", value);
                    _menuSize = value;
                }
            }
        }
        private static float? _menuSize;

        [UIValue("song-rain")]
        public static bool songRain
        {
            get
            {
                if (_songRain == null)
                {
                    _songRain = Plugin.config.GetBool("Settings", "SongRain", true, true);
                }
                return _songRain.Value;
            }
            set
            {
                if (value != _songRain.Value)
                {
                    Plugin.config.SetBool("Settings", "SongRain", value);
                    _songRain = value;
                }
            }
        }
        private static bool? _songRain;

        [UIValue("size-in-song")]
        public static float songSize {
            get {
                if(_songSize == null) {
                    _songSize = Plugin.config.GetFloat("Settings", "SongSize", 0.6f, true);
                }
                return _songSize.Value;
            }
            set {
                if(value != _songSize.Value) {
                    Plugin.config.SetFloat("Settings", "SongSize", value);
                    _songSize = value;
                }
            }
        }
        private static float? _songSize;

        [UIValue("emote-delay")]
        public static int emoteDelay {
            get {
                if(_emoteDelay == null) {
                    _emoteDelay = Plugin.config.GetInt("Settings", "EmoteDelay", 8, true);
                }
                return _emoteDelay.Value;
            }
            set {
                if(value != _emoteDelay.Value) {
                    Plugin.config.SetInt("Settings", "EmoteDelay", value);
                    _emoteDelay = value;
                }
            }
        }
        private static int? _emoteDelay;
    }
}
