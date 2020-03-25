using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoteRain
{
    abstract class Command
    {
        public abstract string regName { get;}
        public abstract string trigger { get;}
        public abstract void onTrigger(string arg);
    }

    class Toggle : Command
    {
        public override string regName 
        { 
            get
            {
                return "ToggleCMD";
            }
        }
        public override string trigger
        {
            get
            {
                return "toggle";
            }
        }

        public override void onTrigger(string arg)
        {
            switch (arg)
            {
                case "off":
                    Settings.menuRain = false;
                    Settings.songRain = false;
                    break;
                case "on":
                    Settings.menuRain = true;
                    Settings.songRain = true;
                    break;
                case "menu":
                    Settings.menuRain = !Settings.menuRain;
                    break;
                case "song":
                    Settings.songRain = !Settings.songRain;
                    break;
                case "all":
                default:
                    if (Settings.menuRain || Settings.songRain) goto case "off";
                    else goto case "on";
            }
        }
    }
}
