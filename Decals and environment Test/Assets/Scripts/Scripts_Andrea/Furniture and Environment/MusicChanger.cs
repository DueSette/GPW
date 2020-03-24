﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicChanger : MonoBehaviour
{
    [SerializeField] AudioClip newBackgroundMusic;
    [SerializeField] float fadeOutTime = 3;
    [SerializeField] float fadeInTime = 3;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(SoundManager.instance.FadeBGM(newBackgroundMusic, fadeOutTime, fadeInTime));
    }

    private void OnTriggerExit(Collider other)
    {
        SoundManager.instance.RestorePreviousBGM(fadeOutTime, fadeInTime);
    }
}
