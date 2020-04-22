﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BadoomNotePickup : NoteInGameObjectScript
{
    [SerializeField] GameObject[] badooms;
    [SerializeField] GameObject[] emissiveMaterialsToSwap;
    [SerializeField] Material materialToBeSwappedIn;

    [SerializeField] ParticleSystem wallParticle;

    [SerializeField] Light[] lightSourcesToTurnOff;
    [SerializeField] GameObject darkPostProcess;

    [SerializeField] OpenableDoor doorToLock;

    private Material originalEmissiveMaterial;
    private float[] originalIntensities;
    
    [Header("Music Changing")]
    [SerializeField] AudioClip newBackgroundMusic;
    [SerializeField] float fadeOutTime = 4;
    [SerializeField] float fadeInTime = 4;

    protected override void OnInteraction()
    {
        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<MeshRenderer>().enabled = false;

        SetupBackupReferences();

        // Locking Player in badoom room
        doorToLock.LockDoor("MemoryReturn");

        // Unlocking door to cake room
        OpenableDoor.OnDoorUnlockEvent("Badooms");

        //Starts badoom sequence
        ActivateBadooms(true);

        //TurnLightsOff(true); ENABLE IF NEEDED (DESIGN CHOICE)
        EnableDarkPostProcess(true);

        wallParticle.Play();

        // Badoom Music
        StartCoroutine(SoundManager.instance.FadeBGM(newBackgroundMusic, fadeOutTime, fadeInTime));

    }

    private void SetupBackupReferences()
    {
        if (materialToBeSwappedIn == null)
            return;

        materialToBeSwappedIn = emissiveMaterialsToSwap[0].GetComponent<Renderer>().material; //take a reference to reinstate the previous emissive materials
        originalIntensities = new float[lightSourcesToTurnOff.Length];

        for (int i = 0; i < lightSourcesToTurnOff.Length; i++)        
            originalIntensities[i] = lightSourcesToTurnOff[i].intensity;     
    }

    public void EndBadoomSequence() //brings everything backtonormal, should happen offscreen
    {
        ActivateBadooms(false);
        //TurnLightsOff(false);
        EnableDarkPostProcess(false);
        wallParticle.Stop();
    }

    void ActivateBadooms(bool start)
    {
        foreach (GameObject bad in badooms)
            if (bad != null)
                bad.SetActive(start);
    }

    void TurnLightsOff(bool start)
    {
        foreach (GameObject emissiveObject in emissiveMaterialsToSwap)
        {
            emissiveObject.GetComponent<Renderer>().material = start ? materialToBeSwappedIn : originalEmissiveMaterial;
        }
        for (int i = 0; i < lightSourcesToTurnOff.Length; i++)
        {
            Light light = lightSourcesToTurnOff[i];
            light.intensity = start ? 0 : originalIntensities[i];
        }
    }

    void EnableDarkPostProcess(bool start)
    {
        darkPostProcess?.SetActive(start);
    }
}
