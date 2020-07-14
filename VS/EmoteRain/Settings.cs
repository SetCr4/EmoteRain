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
        [UIAction("test-subrain")]
        private void TestSubRainClickAction() => RequestCoordinator.subRain();

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
                    _menuSize = (float)Plugin.config.GetInt("Settings", "MenuSize", 4, true)/10;
                }
                return _menuSize.Value;
            }
            set
            {
                if (value != _menuSize.Value)
                {
                    Plugin.config.SetInt("Settings", "MenuSize", (int)(value*10));
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
                    _songSize = (float)Plugin.config.GetInt("Settings", "SongSize", 6, true)/10;
                }
                return _songSize.Value;
            }
            set {
                if(value != _songSize.Value) {
                    Plugin.config.SetInt("Settings", "SongSize", (int)(value*10));
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
        
        [UIValue("emote-fallspeed")]
        public static float emoteFallspeed {
            get {
                if(_emoteFallspeed == null) {
                    _emoteFallspeed = (float)Plugin.config.GetInt("Settings", "EmoteFallspeed", 30, true)/10;
                }
                return _emoteFallspeed.Value;
            }
            set {
                if(value != _emoteFallspeed.Value) {
                    Plugin.config.SetInt("Settings", "EmoteFallspeed", (int)(value * 10));
                    _emoteFallspeed = value;
                }
            }
        }
        private static float? _emoteFallspeed;

        [UIValue("sub-rain")]
        public static bool subRain
        {
            get
            {
                if (_subRain == null)
                {
                    _subRain = Plugin.config.GetBool("Settings", "SubRain", true, true);
                }
                return _subRain.Value;
            }
            set
            {
                if (value != _subRain.Value)
                {
                    Plugin.config.SetBool("Settings", "SubRain", value);
                    _subRain = value;
                }
            }
        }
        private static bool? _subRain;

        public static string subrainEmotes {
            get {
                if(_subrainEmotes == null) {
                    _subrainEmotes = Plugin.config.GetString("Settings", "SubrainEmotes", "", true);
                }
                return _subrainEmotes;
            }
            set {
                if(value != _subrainEmotes) {
                    Plugin.config.SetString("Settings", "SubrainEmotes", value);
                    _subrainEmotes = value;
                }
            }
        }
        private static string _subrainEmotes;

        [UIValue("subrain-emotecount")]
        public static int subrainEmotecount
        {
            get
            {
                if (_subrainEmotecount == null)
                {
                    _subrainEmotecount = Plugin.config.GetInt("Settings", "SubrainEmotecount", 20, true);
                }
                return _subrainEmotecount.Value;
            }
            set
            {
                if (value != _subrainEmotecount.Value)
                {
                    Plugin.config.SetInt("Settings", "SubrainEmotecount", value);
                    _subrainEmotecount = value;
                }
            }
        }
        private static int? _subrainEmotecount;

        //This is just here if the prefix should be customizeable later on
        //Or in other words: Doesn't do anything right now
        public static string prefix 
        { 
            get 
            { 
                if(_prefix == null) 
                {
                    //_prefix = Plugin.config.GetString("Settings", "Prefix", "!er", true);
                    _prefix = "!er";
                }
                return _prefix;
            }
            set 
            {
                if (value != _prefix)
                {
                    //Plugin.config.SetString("Settings", "Prefix", value);
                    _prefix = value;
                }
            }
        }
        private static string _prefix;
    }
}
