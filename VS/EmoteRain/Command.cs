using StreamCore.SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmoteRain
{
    abstract class Command
    {
        public abstract string regName { get;}
        public abstract string trigger { get;}
        public abstract void onTrigger(string[] arg);
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

        public override void onTrigger(string[] arg)
        {
            switch (arg[0])
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

    class Ban : Command
    {
        public override string regName 
        {
            get
            {
                return "BanCMD";
            }
        }

        public override string trigger
        {
            get
            {
                return "ban";
            }
        }

        public override void onTrigger(string[] arg)
        {
            //building the webrequest
            string weburl = "https://api.twitch.tv/kraken/users?login=" + arg[0];
            WebRequest request = WebRequest.Create(weburl);
            request.Headers.Add("Accept", "application/vnd.twitchtv.v5+json");
            request.Headers.Add("Client-ID", "1h89o1f9o925i7foabk75y1qa78vjx");
            WebResponse response = request.GetResponse();
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            JSONNode clientJson = JSON.Parse(sr.ReadToEnd());
            clientJson = clientJson["users"];
            clientJson = clientJson[0];
            string userId = clientJson["_id"].Value;

            //add UserID to banned list

        }
    }
    
    class Unban : Command
    {
        public override string regName 
        {
            get
            {
                return "UnbanCMD";
            }
        }

        public override string trigger
        {
            get
            {
                return "unban";
            }
        }

        public override void onTrigger(string[] arg)
        {
            foreach(string userToUnban in arg)
            {
                //search up UserID from username in twitchapi

                //remove UserID from banned list if it exists

            }
        }
    }
}
