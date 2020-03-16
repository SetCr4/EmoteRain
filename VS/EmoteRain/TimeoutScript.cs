using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoteRain {
    internal class TimeoutScript:MonoBehaviour {
        [SerializeField]
        private float timeLimit = 15.0f;
        private float timeoutTimer = 0;
        internal string key;
        internal Mode mode;
        private bool ready;
        private ushort queue;
        private byte frameCount;
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

        internal void Emit(ushort amount) {
            if(amount > 0) {
                queue += amount;
                StartCoroutine("Emit");
            }
        }
        private IEnumerator<WaitForFixedUpdate> Emit() {
            if(!ready) {
                yield return new WaitForFixedUpdate();
                ready = true;
            }
            if(queue > 0) {
                timeoutTimer = 0;
                if(frameCount >= Settings.emoteDelay) {
                    frameCount = 0;
                    PS.Emit(1);
                }
                frameCount++;
            } else {
                frameCount = 0;
                timeoutTimer += Time.fixedDeltaTime;
                if(timeoutTimer > timeLimit) {
                    RequestCoordinator.UnregisterPS(key, mode);
                }
            }
        }
    }
}
