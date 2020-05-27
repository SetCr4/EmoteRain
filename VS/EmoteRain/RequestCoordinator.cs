using ChatCore.Interfaces;
using ChatCore.Models.Twitch;
using EnhancedStreamChat.Chat;
using EnhancedStreamChat.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EmoteRain.Logger;
using PS_Prefab_Pair = System.ValueTuple<System.Collections.Generic.Dictionary<string, EmoteRain.TimeoutScript>, UnityEngine.GameObject>;

namespace EmoteRain
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public static class RequestCoordinator
    {
        //private static GameObject prefabMenu;
        //private static GameObject prefabPlay;

        private static Mode mode;

        internal static Action<IChatEmote, byte> EmoteQueue;

        private static Dictionary<Mode, PS_Prefab_Pair> particleSystems = new Dictionary<Mode, PS_Prefab_Pair>();

        internal static Scene myScene
        {
            get
            {
                if (!_myScene.HasValue)
                {
                    _myScene = SceneManager.CreateScene("EmoteRainScene");
                }
                return _myScene.Value;
            }
        }

        private static Scene? _myScene;

        internal static void OnLoad()
        {
            Log("in OnLoad()");
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("EmoteRain.emoterain"));
            //assetBundle.LoadAllAssets();
            EmoteQueue = MessageCallback;

            particleSystems[Mode.Menu] =
                new PS_Prefab_Pair(
                    new Dictionary<string, TimeoutScript>(),
                    assetBundle.LoadAsset<GameObject>("ERParticleSystemMenu Variant")
                )
            ;
            Log("Prefab at: " + (particleSystems[Mode.Menu].Item2 ? particleSystems[Mode.Menu].Item2.GetFullPath() : "null"));
            particleSystems[Mode.Play] =
                new PS_Prefab_Pair(
                    new Dictionary<string, TimeoutScript>(),
                    assetBundle.LoadAsset<GameObject>("ERParticleSystemPlaySpace Variant")
                )
            ;
            Log("Prefab at: " + (particleSystems[Mode.Play].Item2 ? particleSystems[Mode.Play].Item2.GetFullPath() : "null"));
        }

        private static void MessageCallback(IChatEmote emote, byte count)
        {
            if ((mode == Mode.Menu && Settings.menuRain) || (mode == Mode.Play && Settings.songRain))
            {
                SharedCoroutineStarter.instance.StartCoroutine(WaitForCollection(emote, count));
            }
        }

        private static IEnumerator<WaitUntil> WaitForCollection(IChatEmote emote, byte count)
        {
            float time = Time.time;

            EnhancedImageInfo enhancedImageInfo = default;
            yield return new WaitUntil(() => ChatImageProvider.instance.CachedImageInfo.TryGetValue(emote.Id, out enhancedImageInfo) && mode != Mode.None);

            //Log($"Continuing after {Time.time - time} seconds...");

            TimeoutScript cloneTimer;
            PS_Prefab_Pair ps_Prefab_Pair = particleSystems[mode];

            if (!ps_Prefab_Pair.Item1.ContainsKey(emote.Id))
            {
                cloneTimer = UnityEngine.Object.Instantiate(ps_Prefab_Pair.Item2).GetComponent<TimeoutScript>();
                var main = cloneTimer.PS.main;
                if (mode == Mode.Menu) main.startSize = Settings.menuSize;
                if (mode == Mode.Play) main.startSize = Settings.songSize;
                main.startSpeed = Settings.emoteFallspeed;
                main.startLifetime = (8 / (Settings.emoteFallspeed - 1)) + 1;
                cloneTimer.key = emote.Id;
                cloneTimer.mode = mode;
                SceneManager.MoveGameObjectToScene(cloneTimer.gameObject, myScene);
                ps_Prefab_Pair.Item1.Add(emote.Id, cloneTimer);

                //sorta working animated emotes
                if (emote.IsAnimated)
                {
                    var tex = cloneTimer.PS.textureSheetAnimation;
                    tex.enabled = true;
                    tex.mode = ParticleSystemAnimationMode.Sprites;
                    tex.timeMode = ParticleSystemAnimationTimeMode.Lifetime;
                    int spriteCount = enhancedImageInfo.AnimControllerData.sprites.Length - 1;
                    float timeForEmote = 0;
                    for (int i = 0; i < spriteCount; i++)
                    {
                        tex.AddSprite(enhancedImageInfo.AnimControllerData.sprites[i]);
                        timeForEmote += enhancedImageInfo.AnimControllerData.delays[i];
                    }

                    //float lifeTime = cloneTimer.PS.main.startLifetime.constant * 1000;
                    AnimationCurve curve = new AnimationCurve();
                    float singleFramePercentage = 1.0f / spriteCount;
                    float currentTimePercentage = 0;
                    float currentFramePercentage = 0;

                    for (int frameCounter = 0; currentTimePercentage <= 1; frameCounter++)
                    {
                        if (frameCounter > spriteCount)
                        {
                            frameCounter = 0;
                            currentFramePercentage = 0;
                        }
                        curve.AddKey(currentTimePercentage, currentFramePercentage);
                        currentTimePercentage += enhancedImageInfo.AnimControllerData.delays[frameCounter] / timeForEmote;
                        currentFramePercentage += singleFramePercentage;
                    }
                    tex.frameOverTime = new ParticleSystem.MinMaxCurve(1.0f, curve);
                    tex.cycleCount = (int)(cloneTimer.PS.main.startLifetime.constant * 1000 / timeForEmote);
                }
                //end of animated emotes

                //Log("Assigning texture...");
                cloneTimer.PSR.material.mainTexture = enhancedImageInfo.Sprite.texture;
            }
            else
            {
                cloneTimer = ps_Prefab_Pair.Item1[emote.Id];
            }

            cloneTimer.Emit(count);

            //Log("ParticleSystems notified! ");

        }
        //Needs rework...
        internal static void EnvironmentSwitched(string scene, SceneLoadMode sceneLoadMode)
        {
            if (mode == Mode.None && scene == "MenuEnvironment" && sceneLoadMode == SceneLoadMode.Load)
            {
                mode = Mode.Menu;
            }
            else if (mode == Mode.Menu)
            {
                if (sceneLoadMode == SceneLoadMode.Load && scene != "MenuEnvironment")
                {
                    mode = Mode.Play;
                }
                else if (sceneLoadMode == SceneLoadMode.Unload && scene == "MenuEnvironment")
                {
                    mode = Mode.None;
                }
            }
            else
            {
                if (sceneLoadMode == SceneLoadMode.Unload)
                {
                    mode = Mode.Menu;
                }
            }
        }

        internal static void UnregisterPS(string key, Mode mode)
        {
            UnityEngine.Object.Destroy(particleSystems[mode].Item1[key]);
            //Log("Inactive ParticleSystem. Removing...");
            particleSystems[mode].Item1.Remove(key);
        }

        internal static void subReceived()
        {
            Log("This should be after a received sub.");
            TimeoutScript cloneTimer;
            PS_Prefab_Pair ps_Prefab_Pair = particleSystems[mode];
            cloneTimer = UnityEngine.Object.Instantiate(ps_Prefab_Pair.Item2).GetComponent<TimeoutScript>();
            var main = cloneTimer.PS.main;
            if (mode == Mode.Menu) main.startSize = Settings.menuSize;
            if (mode == Mode.Play) main.startSize = Settings.songSize;
            main.startSpeed = Settings.emoteFallspeed;
            main.startLifetime = (8 / (Settings.emoteFallspeed - 1)) + 1;
            cloneTimer.key = "IReallyHopeNoOneWillEverUseThisKeyAsALegitEmoteName";
            cloneTimer.mode = mode;
            ps_Prefab_Pair.Item1.Add(cloneTimer.key, cloneTimer);
            SceneManager.MoveGameObjectToScene(cloneTimer.gameObject, myScene);

            if (ps_Prefab_Pair.Item1.ContainsKey(cloneTimer.key))
            {
                cloneTimer = ps_Prefab_Pair.Item1[cloneTimer.key];
            }

            cloneTimer.Emit(20);
        }
    }
}
