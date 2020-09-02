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

namespace EmoteRain.Commands
{
    abstract class ERCommand
    {
        public abstract string regName { get; }
        public abstract string trigger { get; }
        public abstract int neededRank { get; } //needed userrank to use that command 
                                                //(Disabled: 0; User: 1; Mods: 2; Broadcaster: 3) 
                                                //Disable is added if the needed rank should be customizeable later on
        public abstract string desc { get; }


        public abstract void onTrigger(IChatService svc, IChatMessage msg, string[] arg);
    }

    class Toggle : ERCommand
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
        public override string desc
        {
            get
            {
                return "toggles rain; available args: on, off, menu, song, ";
            }
        }

        public override void onTrigger(IChatService svc, IChatMessage msg, string[] arg)
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
            svc.SendTextMessage(outputMsg, msg.Channel);
        }
    }

    class Alert : ERCommand
    {
        public override string regName 
        { 
            get
            {
                return "AlertTestCMD";
            }
        }
        public override string trigger
        {
            get
            {
                return "alert";
            }
        }
        public override int neededRank
        {
            get
            {
                return 2; //Mod and upwards
            }
        }
        public override string desc
        {
            get
            {
                return "Debug-Command";
            }
        }

        public override void onTrigger(IChatService svc, IChatMessage msg, string[] arg)
        {
            if (arg.Length < 1)
            {
                arg = new string[] { "" };
            }

            switch (arg[0])
            {
                case "sub":
                    svc.SendTextMessage("Sent Debug Sub Rain.", msg.Channel);
                    RequestCoordinator.subRain();
                    break;
                case "":
                    svc.SendTextMessage($"No Alert Rain selected. {Settings.prefix} alert <type>", msg.Channel);
                    break;
            }
        }
    }

    class Help : ERCommand
    {
        public override string regName
        {
            get
            {
                return "HelpCMD";
            }
        }
        public override string trigger
        {
            get
            {
                return "help";
            }
        }
        public override int neededRank
        {
            get
            {
                return 1; //user
            }
        }
        public override string desc
        {
            get
            {
                return "displays this helppage";
            }
        }

        public override void onTrigger(IChatService svc, IChatMessage msg, string[] arg)
        {
            string outputMsg = "/me [EmoteRain-Helppage] ";
            
            foreach(ERCommand e in CommandRegistration.registeredCommands.Values)
            {
                if(e.neededRank > 0)
                    outputMsg += $"{Settings.prefix} {e.trigger} - {e.desc} | ";
            }
            outputMsg += "Checkout https://github.com/SetCr4/EmoteRain/blob/master/README.md for more help!";
            svc.SendTextMessage(outputMsg, msg.Channel);
        }
    }
    
    class SetEvent : ERCommand
    {
        public override string regName
        {
            get
            {
                return "SetEventCMD";
            }
        }
        public override string trigger
        {
            get
            {
                return "setevent";
            }
        }
        public override int neededRank
        {
            get
            {
                return 3; //Broadcaster
            }
        }
        public override string desc
        {
            get
            {
                return $"Sets the Emotes used when someone subs. Use {Settings.prefix} {trigger} <event> <emotes>. Leave <emotes> empty for default.";
            }
        }

        public override void onTrigger(IChatService svc, IChatMessage msg, string[] arg)
        {
            string emoteIds = "";
            foreach(IChatEmote e in msg.Emotes)
            {
                if(!e.IsAnimated)
                    emoteIds += $"{e.Id} ";
            }

            while(emoteIds.EndsWith(" "))
            {
                emoteIds = emoteIds.Substring(0, emoteIds.Length - 1);
            }

            if (arg.Length < 1)
            {
                arg = new string[] { "" };
            }

            switch (arg[0])
            {
                case "sub":
                    Settings.subrainEmotes = emoteIds;
                    if (emoteIds.Equals(""))
                        svc.SendTextMessage("Subrain set back to default <3", msg.Channel);
                    else
                        svc.SendTextMessage($"Subrain set with {emoteIds.Split(' ').Length} Emotes!", msg.Channel);
                    break;
                case "":
                    svc.SendTextMessage($"No Event selected. Use {Settings.prefix} {trigger} <event> <emotes>", msg.Channel);
                    break;
                default:
                    svc.SendTextMessage($"Selected Event not recognized. Use {Settings.prefix} {trigger} <event> <emotes>", msg.Channel);
                    break;
            }
        }
    }
}
