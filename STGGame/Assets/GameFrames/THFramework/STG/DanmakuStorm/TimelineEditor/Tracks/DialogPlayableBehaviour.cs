
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

namespace THGame
{
    [Serializable]
    public class DialogPlayableBehaviour : PlayableBehaviour
    {
        private GameObject obj;
        public string str;

        public GameObject Obj
        {
            get
            {
                return obj;
            }

            set
            {
                obj = value;
            }
        }

        // Called when the owning graph starts playing
        public override void OnGraphStart(Playable playable)
        {

        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(Playable playable)
        {

        }

        // Called when the state of the playable is set to Play
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {

        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {

        }

        // Called each frame while the state is set to Play
        public override void PrepareFrame(Playable playable, FrameData info)
        {

            //Debug.Log(Obj.name);
        }
    }

}

