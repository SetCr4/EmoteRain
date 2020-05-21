using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatCore.SimpleJSON;
using ChatCore.Services.Twitch;
using ChatCore.Models.Twitch;
using ChatCore.Interfaces;

namespace EmoteRain
{
    abstract class Command
    {
        public abstract string regName { get;}
        public abstract string trigger { get;}
        public abstract int neededRank { get; } //needed userrank to use that command 
                                                //(Disabled: 0; User: 1; Mods: 2; Broadcaster: 3) 
                                                //Disable is added if the needed rank should be customizeable later on

        public abstract void onTrigger(IChatService svc, IChatChannel channel, string[] arg);
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
        public override int neededRank
        {
            get
            {
                return 2; //Mods and upwards
            }
        }

        public override void onTrigger(IChatService svc, IChatChannel channel, string[] arg)
        {
            string outputMsg = "Something went wrong D:";
            if(arg.Length < 1)
            {
                arg = new string[]{""};
            }

            switch (arg[0])
            {
                case "off":
                    Settings.menuRain = false;
                    Settings.songRain = false;
                    outputMsg = "Rain toggled off";
                    break;
                case "on":
                    Settings.menuRain = true;
                    Settings.songRain = true;
                    outputMsg = "Rain toggled on";
                    break;
                case "menu":
                    Settings.menuRain = !Settings.menuRain;
                    outputMsg = $"Rain in menu toggled {(Settings.menuRain ? "on" : "off")}";
                    break;
                case "song":
                    Settings.songRain = !Settings.songRain;
                    outputMsg = $"Rain in menu toggled {(Settings.songRain ? "on" : "off")}";
                    break;
                case "all":
                default:
                    if (Settings.menuRain || Settings.songRain) goto case "off";
                    else goto case "on";
            }
            svc.SendTextMessage(outputMsg,channel);
        }
    }
}
