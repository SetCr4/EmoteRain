using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChatCore.Services.Twitch;
using static EmoteRain.Logger;
using ChatCore;
using ChatCore.Interfaces;
using System.Reflection;

namespace EmoteRain {
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.<br/>
    /// For a full list of Messages a Monobehaviour can receive from the game,<br/>see <seealso cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>.
    /// </summary>
    internal class TwitchMSGHandler {

        private static ChatCoreInstance sc;

        public static void onLoad()
        {
            registerCommands();
            SharedCoroutineStarter.instance.StartCoroutine(CheckChat());
        }

        private static IEnumerator CheckChat()
        {
            yield return new WaitForSeconds(1);
            sc = ChatCoreInstance.Create();
            var svc = sc.RunTwitchServices();
            svc.OnTextMessageReceived += Svc_OnTextMessageReceived;
        }

        private static void Svc_OnTextMessageReceived(IChatService svc, IChatMessage msg)
        {
            //Log($"MSG from {msg.Sender.Name} is User: {!msg.Sender.IsBroadcaster&&!msg.Sender.IsModerator}; is Mod: {msg.Sender.IsModerator}; is BC: {msg.Sender.IsBroadcaster}");
            if (msg.Message.StartsWith(Settings.prefix))
                CMDHandler(svc, msg);
            else
                MSGHandler(msg);
        }

        private static void MSGHandler(IChatMessage twitchMsg) {
            //Log("Got Twitch Msg!\nMessage: " + twitchMsg.Message);
            IChatEmote[] emoteTag = twitchMsg.Emotes; //remove filter when working with animated emotes
            if(emoteTag.Length > 0) {
                //Log($"Sending {emoteTag.Length} Emotes to Emote-Queue...");
                queueEmoteSprites(emoteTag);
            } 
        }
        
        private static void CMDHandler(IChatService svc, IChatMessage twitchMsg) {
            string[] msgSplited = twitchMsg.Message.Split(' ');
            Command commandToExecute = null;
            if(registeredCommands.TryGetValue(msgSplited[1], out commandToExecute))
            {
                if (isAllowed(commandToExecute.neededRank, twitchMsg.Sender))
                {
                    commandToExecute.onTrigger(svc, twitchMsg.Channel, msgSplited.TakeLast(msgSplited.Length - 2).ToArray());
                }
                else
                {
                    svc.SendTextMessage("[EmoteRain] You are not allowed to use this Command!", twitchMsg.Channel);
                }
            }
        }

        private static void queueEmoteSprites(IChatEmote[] unstackedEmotes) {
            (from iChatEmote in unstackedEmotes
                group iChatEmote by iChatEmote.Id into emoteGrouping
                select new { emote = emoteGrouping.First(), count = (byte)emoteGrouping.Count() }
            ).ToList().ForEach(x => HMMainThreadDispatcher.instance.Enqueue(EnqueueEmote(x.emote, x.count)));

        }

        private static IEnumerator EnqueueEmote(IChatEmote emote, byte count) {
            yield return null;
            RequestCoordinator.EmoteQueue(emote, count);
        }

        private static IChatEmote[] filterAnimated(IChatEmote[] unfilteredEmotes, bool anim = false)
        {
            List<IChatEmote> filteredEmotes = new List<IChatEmote>();
            foreach(IChatEmote e in unfilteredEmotes)
            {
                if(e.IsAnimated == anim) filteredEmotes.Add(e);
            }
            return filteredEmotes.ToArray();
        }

        private static Dictionary<String, Command> registeredCommands = new Dictionary<string, Command>();
        private static void registerCommands()
        {
            IEnumerable<Command> commands = Extensions.GetEnumerableOfType<Command>();
            foreach(Command e in commands)
            {
                registeredCommands.Add(e.trigger,e);
            }
        }

        private static bool isAllowed(int neededRank, IChatUser user)
        {
            //Disabled: 0; User: 1; Mods: 2; Broadcaster: 3
            bool returner = false;
            switch(neededRank)
            {
                case 0:
                    break;
                case 1:
                    returner = true;
                    break;
                case 2:
                    if (user.IsModerator || user.IsBroadcaster)
                        returner = true;
                    break;
                case 3:
                    if (user.IsBroadcaster)
                        returner = true;
                    break;
            }
            return returner;
        }
    }
}
