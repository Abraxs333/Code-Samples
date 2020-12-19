using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class Soundeffects : MonoBehaviour
{
    public Sound[] sounds;
    public static Soundeffects instance;
    //public GameObject PauseMenu;

    private void Awake()
    {

       

        foreach (Sound s in sounds)
        {
           s.source= gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("Theme");
       // PauseMenu.SetActive(false);
    }

    public void Play (string name)
    {
       Sound s= Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.Log("Audioclip:  " + name + "not found");
            return;
        }
        s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audioclip:  " + name + "not found");
            return;
        }
        s.source.Pause();
    }

    public void UnPause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audioclip:  " + name + "not found");
            return;
        }
        s.source.UnPause();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audioclip:  " + name + "not found");
            return;
        }
        s.source.Stop();
    }

}
