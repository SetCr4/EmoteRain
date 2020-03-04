using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using static EmoteRain.Logger;

namespace EmoteRain
{

    internal class Plugin : IBeatSaberPlugin
    {
        private static bool init;

        internal static string Name => "EmoteRain";

        public void Init(IPALogger logger)
        {
            Logger.Init(logger);
            Log("Logger initialized.");
        }

        /// <summary>
        /// Called when the a scene's assets are loaded.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sceneMode"></param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (!init)
            {
                init = true;
                SharedCoroutineStarter.instance.StartCoroutine(WaitForMenu());
            }
            if(scene.name.Contains("Environment"))
            {
                RequestCoordinator.EnvironmentSwitched(scene.name,SceneLoadMode.Load);
            }
        }
        private static IEnumerator<WaitUntil> WaitForMenu()
        {
            yield return new WaitUntil(() => {
                Scene scene1, scene2, scene3, scene4;
                scene1 = SceneManager.GetSceneByName("MenuCore");
                scene2 = SceneManager.GetSceneByName("MenuEnvironment");
                scene3 = SceneManager.GetSceneByName("MainMenu");
                scene4 = SceneManager.GetSceneByName("MenuViewControllers");
                return
                    scene1.isLoaded && scene1.IsValid() &&
                    scene2.isLoaded && scene2.IsValid() &&
                    scene3.isLoaded && scene3.IsValid() &&
                    scene4.isLoaded && scene4.IsValid();
            });
            Init();
        }

        private static void Init()
        {
            RequestCoordinator.OnLoad();
        }

        public void OnApplicationStart(){}
        public void OnApplicationQuit(){}
        public void OnFixedUpdate(){}
        public void OnUpdate(){}
        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene){}
        public void OnSceneUnloaded(Scene scene){
            if (scene.name.Contains("Environment"))
            {
                RequestCoordinator.EnvironmentSwitched(scene.name, SceneLoadMode.Unload);
            }
        }
    }
}
