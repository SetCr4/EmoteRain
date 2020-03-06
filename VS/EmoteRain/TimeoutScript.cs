using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoteRain
{
    internal class TimeoutScript : MonoBehaviour
    {
        [SerializeField]
        private float timeLimit = 15.0f;
        private float timeoutTimer = 0;
        internal string key;
        internal Mode mode;
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



        private void Update()
        {
            timeoutTimer += Time.deltaTime;
            if (timeoutTimer > timeLimit)
            {
                RequestCoordinator.UnregisterPS(key, mode);
            }
        }

        internal void resetTimer() => timeoutTimer = 0;
    }
}
