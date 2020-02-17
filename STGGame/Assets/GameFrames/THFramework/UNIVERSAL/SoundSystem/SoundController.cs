using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundController : MonoBehaviour
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

        public float Speed
        {
            get { return audio.volume; }
            set { audio.volume = value; }
        }

        public int NormalizedTime
        {
            get { return 0; }
        }
        
        //
        public void Play(AudioClip clip, SoundArgs args = null)
        {
            Play();
        }

        public void Play()
        {
            if (args != null)
            {
                
            }
        }

        public void Stop()
        {

        }

        public void Pause(float fadeOut = 0f)
        {

        }

        public void Resume(float fadeIn = 0f)
        {

        }
        //
        private void Awake()
        {
            if (audio == null)
            {
                audio = gameObject.AddComponent<AudioSource>();
            }
        }

    }


}
