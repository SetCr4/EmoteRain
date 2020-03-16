using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EmoteRain.Logger;

namespace EmoteRain {
    internal class TimeoutScript:MonoBehaviour {
        [SerializeField]
        private float timeLimit = 15.0f;
        private float timeoutTimer = 0;
        internal string key;
        internal Mode mode;
        private bool ready;
        private byte queue;
        private byte frameCount = (byte)Settings.emoteDelay;
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
                StartCoroutine(Emit());
            }
        }
        private IEnumerator<WaitForFixedUpdate> Emit() {
            while(true) {
                if(!ready) {
                    yield return new WaitForFixedUpdate();
                    ready = true;
                }
                if(queue > 0) {
                    timeoutTimer = 0;
                    if(frameCount >= Settings.emoteDelay) {
                        frameCount = 0;
                        PS.Emit(1);
                        queue--;
                    }
                    frameCount++;
                    yield return new WaitForFixedUpdate();
                } else {
                    frameCount = 0;
                    timeoutTimer += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                    if(timeoutTimer > timeLimit) {
                        RequestCoordinator.UnregisterPS(key, mode);
                        yield break;
                    }
                }
            }
        }
    }
}
