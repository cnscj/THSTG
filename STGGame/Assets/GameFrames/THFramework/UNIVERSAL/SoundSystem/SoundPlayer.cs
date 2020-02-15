using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundPlayer : MonoBehaviour
    {
        //音频源控件
        public new AudioSource audio;
        public SoundData data;

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
        public void Dispose()
        {
            Destroy(gameObject);
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
