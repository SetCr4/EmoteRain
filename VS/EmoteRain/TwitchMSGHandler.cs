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

namespace EmoteRain {
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.<br/>
    /// For a full list of Messages a Monobehaviour can receive from the game,<br/>see <seealso cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>.
    /// </summary>
    internal class TwitchMSGHandler {

        private static ChatCoreInstance sc;

        public static void onLoad()
        {
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
            //don't need svc yet because only twitch is supported
            MSGHandler(msg);
        }

        private static void MSGHandler(IChatMessage twitchMsg) {
            //Log("Got Twitch Msg!\nMessage: " + twitchMsg.Message);
            IChatEmote[] emoteTag = filterAnimated(twitchMsg.Emotes); //remove filter when working with animated emotes
            if(emoteTag.Length > 0) {
                //Log($"Sending {emoteTag.Length} Emotes to Emote-Queue...");
                queueEmoteSprites(emoteTag);
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
    }
}
