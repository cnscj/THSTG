using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundData : MonoBehaviour
    {
        //音频源控件
        public new AudioSource audio;
        public AudioClip clip;
        public SoundArgs args;


        public AudioSource GetAudio()
        {
            return audio;
        }

        public bool IsPlaying
        {
            get
            {
                return audio != null && audio.isPlaying;
            }
        }

        public bool Mute
        {
            get { return audio.mute; }
            set { audio.mute = value; }
        }

        public float Volume
        {
            get { return audio.volume; }
            set { audio.volume = value; }
        }

        public int NormalizedTime
        {
            get { return 0; }
        }


        //
        public void Dispose()
        {
            Destroy(gameObject);
        }
       
        //
        private void Awake()
        {
            if (audio == null)
            {
                audio = gameObject.AddComponent<AudioSource>();
            }
        }
        private void Start()
        {
            
        }
    }


}
