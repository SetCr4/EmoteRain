﻿using System;
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

        //public TwitchMSGHandler() {
        //    sc = ChatCoreInstance.Create();
        //    var svc = sc.RunTwitchServices();
        //    svc.OnTextMessageReceived += Svc_OnTextMessageReceived;
        //}

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
            Log("Got Twitch Msg!\nMessage: " + twitchMsg.Message);
            //string et = getEmoteTagFromMsg(twitchMsg.rawMessage);
            //string msg = twitchMsg.message;
            //string[] eids = combineAllIDs(getTwitchEmoteIDsFromTag(et),getBTTVEmoteIDsFromMsg(msg),getFFZEmoteIDsFromMsg(msg));
            IChatEmote[] emoteTag = twitchMsg.Emotes;
            List<string> eids = new List<string>();
            if(emoteTag.Length > 0) {
                Log("EmoteIDs:");
                foreach(IChatEmote e in emoteTag) {
                    Log("Emote ID {" + e.Id + "}");
                    eids.Add(e.Id);
                }
                Log($"Sending {emoteTag.Length} Emotes to Emote-Queue...");
                queueEmoteSprites(emoteTag);
            } else {
                Log("No Emotes in msg to queue!");
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

        //private static string getEmoteTagFromMsg(string msg) {
        //    //  bspMsg for msg where "moepHi" is an emote
        //    //      MessageTest2 Emote: moepHi
        //    // 
        //    //  rawMsg is 
        //    //      @badge-info=;badges=broadcaster/1,premium/1;color=#000F92;display-name=Cr4sher_;emotes=301242724:20-25;flags=;id=4fe7c5d4-a9f4-4190-8d7d-6ff55d75ab80;mod=0;room-id=133537093;
        //    //      subscriber=0;tmi-sent-ts=1582742897726;turbo=0;user-id=133537093;user-type= :cr4sher_!cr4sher_@cr4sher_.tmi.twitch.tv PRIVMSG #cr4sher_ :MessageTest2 Emote: moepHi
        //    //
        //    //  therefore "emotes=301242724:20-25" should be extracted

        //    string[] tags = msg.Split(';');
        //    for(int i = 0; i < tags.Length; i++) {
        //        if(tags[i].StartsWith("emotes=")) {
        //            return tags[i];
        //        }
        //    }
        //    return "emotes=";
        //}

        //private static string[] getBTTVEmoteIDsFromMsg(string msg)
        //{
        //    List<string> EmoteIDList = new List<string>();
        //    string[] words = msg.Split(' ');
        //    foreach(string e in words)
        //    {
        //        string str = "";
        //        ImageDownloader.BTTVEmoteIDs.TryGetValue(e,out str);
        //        if (str != null) EmoteIDList.Add("B" + str);
        //    }
        //    return EmoteIDList.ToArray();
        //}

        //private static string[] getFFZEmoteIDsFromMsg(string msg)
        //{
        //    List<string> EmoteIDList = new List<string>();
        //    string[] words = msg.Split(' ');
        //    foreach (string e in words)
        //    {
        //        string str = "";
        //        ImageDownloader.FFZEmoteIDs.TryGetValue(e, out str);
        //        if (str != null) EmoteIDList.Add("F" + str);
        //    }
        //    return EmoteIDList.ToArray();
        //}

        //private static string[] getTwitchEmoteIDsFromTag(string emoteIDs)
        //{
        //    //  bspInput
        //    //      emotes=301290052:29-35/301242724:37-42/115845:44-52/106293:54-60
        //    //  
        //    //  bspOutput
        //    //      {"301290052","301242724","115845","106293"}

        //    List<string> emoteIDArray = new List<string>();

        //    if(emoteIDs.Split('=')[1] == "") return emoteIDArray.ToArray();
        //    Log(emoteIDs.Split('=').Length);
        //    string[] emotesWithIndex = emoteIDs.Split('=')[1].Split('/');
        //    foreach(string e in emotesWithIndex) {
        //        string currentEmoteID = e.Split(':')[0];
        //        string[] inLine = e.Split(':')[1].Split(',');
        //        foreach(var _ in inLine) {
        //            emoteIDArray.Add("T" + currentEmoteID);
        //        }
        //    }
        //    return emoteIDArray.ToArray();
        //}

        //private static string[] combineAllIDs(params string[][] EmoteIDArr)
        //{
        //    List<string> combined = new List<string>();
        //    foreach (string[] e in EmoteIDArr)
        //    {
        //        foreach (string e2 in e)
        //        {
        //            combined.Add(e2);
        //        }
        //    }
        //    return combined.ToArray();
        //}
    }
}
