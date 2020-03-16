using System.Collections.Generic;
using UnityEngine;
using static EmoteRain.Logger;

namespace EmoteRain {
    internal class TimeoutScript:MonoBehaviour {
        [SerializeField]
        private float timeLimit = 15.0f;
        internal string key;
        internal Mode mode;
        private byte queue;
        private bool timingOut;
        private IEnumerator<YieldInstruction> coroutine;
        internal ParticleSystem PS {
            get {
                if(_PS == null) {
                    _PS = gameObject.GetComponent<ParticleSystem>();
                }
                return _PS;
            }
        }
        private ParticleSystem _PS;
        internal ParticleSystemRenderer PSR {
            get {
                if(_PSR == null) {
                    _PSR = gameObject.GetComponent<ParticleSystemRenderer>();
                }
                return _PSR;
            }
        }
        private ParticleSystemRenderer _PSR;

        internal void Emit(byte amount) {
            if(amount > 0) {
                queue += amount;
                if(coroutine == null) {
                    coroutine = Init();
                    StartCoroutine(coroutine);
                }else if(timingOut){
                    StopCoroutine(coroutine);
                    timingOut = false;
                    coroutine = Emit();
                    StartCoroutine(coroutine);
                }
            }
        }
        private IEnumerator<WaitForEndOfFrame> Init() {
            yield return new WaitForEndOfFrame();
            coroutine = Emit();
            StartCoroutine(coroutine);
            yield break;
        }
        private IEnumerator<WaitForFixedUpdate> Emit() {
            byte frameCount = (byte)Settings.emoteDelay;
            while(queue > 0) {
                if(frameCount >= Settings.emoteDelay) {
                    frameCount = 0;
                    PS.Emit(1);
                    queue--;
                }
                frameCount++;
                yield return new WaitForFixedUpdate();
            }
            coroutine = TimeOut();
            StartCoroutine(coroutine);
            yield break;
        }
        private IEnumerator<WaitForSeconds> TimeOut() {
            timingOut = true;
            yield return new WaitForSeconds(timeLimit);
            RequestCoordinator.UnregisterPS(key, mode);
            yield break;
        }
    }
}
