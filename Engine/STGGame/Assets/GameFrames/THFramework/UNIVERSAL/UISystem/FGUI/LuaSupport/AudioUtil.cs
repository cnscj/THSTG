using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    public static class AudioUtil
    {

        public static AudioClip ByteToWavAudioClip(byte []data)
        {
            return WavUtility.ToAudioClip(data);
        }

        public static AudioClip ByteToOggAudioClip(byte[] data)
        {
            return default;
        }
    }


}
