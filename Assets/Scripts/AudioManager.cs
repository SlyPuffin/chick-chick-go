using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour, IGameManager {
    public ManagerStatus status {get; private set;}
    public Sound[] sounds;

    public void Startup() {
        Debug.Log("Audio manager starting up");

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }

        status = ManagerStatus.Started;
    }

    public void play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && !s.source.isPlaying) {
            s.source.Play();
        }
    }
    public void stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && s.source.isPlaying) {
            s.source.Stop();
        }
    }
}
