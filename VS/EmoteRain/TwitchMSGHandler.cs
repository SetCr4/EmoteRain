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
using ChatCore.Services;
using System.Reflection;
using EmoteRain.Commands;

namespace EmoteRain {
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.<br/>
    /// For a full list of Messages a Monobehaviour can receive from the game,<br/>see <seealso cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>.
    /// </summary>
    internal class TwitchMSGHandler {

        private static ChatCoreInstance sc;

        public static void onLoad()
        {
            CommandRegistration.registerCommands();
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
            if (msg.Message.StartsWith(Settings.prefix))
            {
                CMDHandler(svc, msg);
                return;
            }
            if(Settings.subRain) SUBHandler(msg);
            MSGHandler(msg);
        }

        private static void SUBHandler(IChatMessage twitchMsg)
        {
            if (!twitchMsg.IsSystemMessage) return;

            Log($"Received System Message: {twitchMsg.Message}");
            if (twitchMsg.Message.StartsWith("⭐") || twitchMsg.Message.StartsWith("👑"))
            {
                Log($"Received System Message: {twitchMsg.Message}; Should be Sub.");
                RequestCoordinator.subRain();
            }
        }

        private static void MSGHandler(IChatMessage twitchMsg) 
        {
            //Log("Got Twitch Msg!\nMessage: " + twitchMsg.Message);
            IChatEmote[] emoteTag = twitchMsg.Emotes; //remove filter when working with animated emotes
            if(emoteTag.Length > 0) {
                //Log($"Sending {emoteTag.Length} Emotes to Emote-Queue...");
                queueEmoteSprites(emoteTag);
            } 
        }
        
        private static void CMDHandler(IChatService svc, IChatMessage twitchMsg) 
        {
            string[] msgSplited = twitchMsg.Message.Split(' ');
            ERCommand commandToExecute = null;
            if(CommandRegistration.registeredCommands.TryGetValue(msgSplited[1], out commandToExecute))
            {
                if (isAllowed(commandToExecute.neededRank, twitchMsg.Sender))
                {
                    commandToExecute.onTrigger(svc, twitchMsg, msgSplited.TakeLast(msgSplited.Length - 2).ToArray());
                }
                else
                {
                    svc.SendTextMessage("[EmoteRain] You are not allowed to use this Command!", twitchMsg.Channel);
                }
            }
        }

        private static void queueEmoteSprites(IChatEmote[] unstackedEmotes) 
        {
            (from iChatEmote in unstackedEmotes
                group iChatEmote by iChatEmote.Id into emoteGrouping
                select new { emote = emoteGrouping.First(), count = (byte)emoteGrouping.Count() }
            ).ToList().ForEach(x => HMMainThreadDispatcher.instance.Enqueue(EnqueueEmote(x.emote, x.count)));

        }

        private static IEnumerator EnqueueEmote(IChatEmote emote, byte count) 
        {
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
