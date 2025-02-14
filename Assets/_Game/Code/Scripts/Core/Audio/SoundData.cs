using System;
using UnityEngine;
using UnityEngine.Audio;

namespace VinhLB
{
    [Serializable]
    public class SoundData
    {
        public AudioClip Clip;
        [HideInInspector]
        public AudioMixerGroup MixerGroup;
        [HideInInspector]
        public bool Loop;
        [HideInInspector]
        public bool PlayOnAwake;
        public float Volume;
    }
}