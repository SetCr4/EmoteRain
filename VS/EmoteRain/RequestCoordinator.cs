using EnhancedStreamChat.Textures;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EmoteRain.Logger;

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

        internal static Action<string> EmoteQueue;

        private static Dictionary<Mode, ValueTuple<Dictionary<string, GameObject>, GameObject>> particleSystems = new Dictionary<Mode, ValueTuple<Dictionary<string, GameObject>, GameObject>>();

        private static Scene myScene
        {
            get
            {
                if (!_myScene.HasValue)
                {
                    Log("Creating Scene..");
                    _myScene = SceneManager.CreateScene("EmoteRainScene");
                }
                Log("Returning Scene..");
                return _myScene.Value;
            }
        }

        private static Scene? _myScene;

        internal static void OnLoad()
        {
            Log("in OnLoad()");
            Log();
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("EmoteRain.emoterain"));
            assetBundle.LoadAllAssets();
            EmoteQueue = MessageCallback;
            Log(assetBundle.GetAllAssetNames());
            Log(assetBundle.GetAllScenePaths());
            Log("------" + assetBundle.LoadAssetWithSubAssets("ERParticleSystem").Length);
            Log();

            particleSystems[Mode.Menu] =
                new ValueTuple<Dictionary<string, GameObject>, GameObject>(new Dictionary<string, GameObject>(),
                assetBundle.LoadAsset<GameObject>("ERParticleSystemMenu Variant"));
            Log(particleSystems[Mode.Menu].Item2 ? particleSystems[Mode.Menu].Item2.GetFullPath() : "null");
            particleSystems[Mode.Play] =
                new ValueTuple<Dictionary<string, GameObject>, GameObject>(new Dictionary<string, GameObject>(),
                assetBundle.LoadAsset<GameObject>("ERParticleSystemPlaySpace Variant"));
            Log(particleSystems[Mode.Play].Item2 ? particleSystems[Mode.Play].Item2.GetFullPath() : "null");
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private static void MessageCallback(string id)
        {
            Log("Received EmoteID: " + id);
            SharedCoroutineStarter.instance.StartCoroutine(WaitForCollection(id));
        }

        private static IEnumerator<WaitUntil> WaitForCollection(string id)
        {
            float time = Time.time;
            Log("Id: " + id);

            CachedSpriteData cachedSpriteData = default;
            yield return new WaitUntil(() => ImageDownloader.CachedTextures.TryGetValue("T" + id, out cachedSpriteData) && mode != Mode.None);

            Log($"Continuing after {Time.time - time} seconds...");

            GameObject prefabClone;
            ValueTuple<Dictionary<string, GameObject>, GameObject> ps_Prefab_Pair = particleSystems[mode];

            if (!ps_Prefab_Pair.Item1.ContainsKey(id))
            {
                Log("Hi");
                Log(ps_Prefab_Pair.Item2 ? ps_Prefab_Pair.Item2.GetFullPath() : "null");
                prefabClone = UnityEngine.Object.Instantiate(ps_Prefab_Pair.Item2);
                Log("Hey");
                prefabClone.GetComponent<TimeoutScript>().key = id;
                prefabClone.GetComponent<TimeoutScript>().mode = mode;
                SceneManager.MoveGameObjectToScene(prefabClone, myScene);
                ps_Prefab_Pair.Item1.Add(id, prefabClone);
                Log("Bye");
            }
            else
            {
                Log("Ho");
                prefabClone = ps_Prefab_Pair.Item1[id];
            }
            prefabClone.GetComponent<TimeoutScript>().resetTimer();
            Log();
            Log("Assigning...");
            prefabClone.GetComponent<ParticleSystemRenderer>().material.mainTexture = cachedSpriteData.sprite.texture;
            //prefabClone.GetComponent<ParticleSystemRenderer>().material = SceneManager.GetSceneByPath("MenuEnvironment").GetRootGameObjects()[0].transform.Find("MenuPlayersPlace/Feet").GetComponent<Renderer>().material;
            Log("Finished assigning!");

            Log("Sprite emited! " + cachedSpriteData.sprite.name);

            SharedCoroutineStarter.instance.StartCoroutine(FuckUnity(prefabClone.GetComponent<ParticleSystem>()));

        }

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

        private static IEnumerator<WaitForEndOfFrame> FuckUnity(ParticleSystem ps)
        {
            yield return new WaitForEndOfFrame();
            ps?.Emit(1);

            Log(ps?.particleCount);
        }
        internal static void UnregisterPS(string key, Mode mode)
        {
            UnityEngine.Object.Destroy(particleSystems[mode].Item1[key]);
            Log("Inactive ParticleSystem. Removing...");
            particleSystems[mode].Item1.Remove(key);
        }
    }
}
