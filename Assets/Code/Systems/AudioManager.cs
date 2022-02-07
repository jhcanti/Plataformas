using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds;

    public Events.EventChangeGameState OnGameStateChanged;


    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleOnGameStateChanged);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }

        GameManager.Instance.UpdateState(GameManager.GameState.INITIALIZING);        
    }
    

    public void Play(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {
            Debug.LogError("Sounds " + _name + "not found");
            return;
        }
        else
        {
            s.source.Play();
        }
    }


    public void Stop(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {
            Debug.LogError("Sounds " + _name + "not found");
            return;
        }
        else
        {
            s.source.Stop();
        }
    }


    public bool isPlaying(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {            
            return false;
        }
        else
        {
            return s.source.isPlaying;
        }
    }


    void HandleOnGameStateChanged(GameManager.GameState state, GameManager.GameState previousState)
    {        
        switch (state)
        {
            case GameManager.GameState.INITIALIZING:                
                break;

            case GameManager.GameState.RUNNING:
                break;

            case GameManager.GameState.DEATH:
                break;

            default:
                break;
        }
    }
}
