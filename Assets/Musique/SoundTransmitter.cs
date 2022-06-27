using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundTransmitter : MonoBehaviour
{
    
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        public bool useRandomVolume = false;
        [Range(0f, 1f)]
        public float volume = 0.1f;
        [Range(0f, 1f)]
        public float volumeRandom = 0.1f;
    
        public bool useRandomPitch = false;
        [Range(-3f, 3f)]
        public float pitch = 1.0f;
        [Range(-3f, 3f)]
        public float pitchRandom = 1.0f;

        public float maxDistance = 20.0f;
    
        public bool playOnAwake = false;
        public bool loop = false;
        public bool in3D = true;
        public float spatialBlend = 0;

        [HideInInspector]
        public AudioSource source;
    }
    
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
            
            s.source.volume = s.volume;
            
            s.source.pitch = s.pitch;

            s.source.maxDistance = s.maxDistance;
            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;
            s.source.spatialize = s.in3D;
            s.source.spatialBlend = s.spatialBlend;
            
            if(s.playOnAwake) Play(s.name);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        if (s.useRandomVolume)
        {
            var minim = Mathf.Max(s.volume, s.volumeRandom);
            var max = Mathf.Min(s.volume, s.volumeRandom);

            s.source.volume = Random.Range(minim, max);
        }
        
        if (s.useRandomPitch)
        {
            var minim = Mathf.Max(s.pitch, s.pitchRandom);
            var max = Mathf.Min(s.pitch, s.pitchRandom);

            s.source.pitch = Random.Range(minim, max);
        }
        
        s.source.Play();
    }

    public IEnumerator PlayWDelay(string name, float time)
    {
        yield return new WaitForSeconds(time);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }
}