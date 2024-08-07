using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip[] musics;
    private AudioSource _audioSource;
    [HideInInspector] public bool isMusic;
    private int _musicIndex;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    private void Update()
    {
        if(!_audioSource.isPlaying && isMusic)
            PlayMusic();
    }

    void PlayMusic()
    {
        _musicIndex++;
        if (_musicIndex >= 3)
            _musicIndex = 0;

        _audioSource.clip = musics[_musicIndex];
        _audioSource.Play();
    }


    public void ChangeVolume(bool isTerminal)
    {
        _audioSource.volume = isTerminal ? .01f : .1f;
    }
}