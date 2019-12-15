﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoriesManager : MonoBehaviour
{
    //=== Class summary ===
    //This class simply manages the two inventories (note inventory and item inventory)
    //It contains functions for opening, closing and browsing the inventories

    public bool noteInventoryOpen = false;
    public bool itemInventoryOpen = false;
    public NoteInventory noteManager;
    public ItemInventory itemManager;
    [SerializeField] AudioClip openInventory;
    [SerializeField] AudioClip navigateInventory;

    void Awake()
    {
        itemManager.gameObject.SetActive(true);
        noteManager.transform.parent.gameObject.SetActive(true); //activates the inventory, which will deactivate itself in the same frame. This loads the note inventory.
    }

    void Update()
    {
        if (GameStateManager.gameState == GameState.IN_GAME || GameStateManager.gameState == GameState.MENU)
            CheckInput();
    }

    void CheckInput()
    {
        //Just input stuff
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (itemInventoryOpen) //if the other inventory is open, close it
                ToggleItemInventoryWindow(false);

            ToggleNoteInventoryWindow();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            if (noteInventoryOpen) //if the other inventory is open, close it
                ForceToggleNoteInventoryWindow(false);

            ToggleItemInventoryWindow();
        }

        if (noteInventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                noteManager.ScrollEntries(false);
                SoundManager.instance.PlaySound(openInventory);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                noteManager.ScrollEntries(true);
                SoundManager.instance.PlaySound(openInventory);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SoundManager.instance.PlaySound(openInventory);
                ToggleNotesDescriptionBox();
            }

        }
        else if (itemInventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                itemManager.ScrollItems(false);
                SoundManager.instance.PlaySound(openInventory);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                itemManager.ScrollItems(true);
                SoundManager.instance.PlaySound(openInventory);
            }
        }
    }

    #region NoteInventory Methods
    void ToggleNoteInventoryWindow()
    {
        if (noteInventoryOpen) //if note inventory was open, reset UI-related stuff and close
        {
            noteManager.CleanPreviousNoteUI();
            noteManager.ScrollEntriesToLast();
            GameStateManager.SetGameState(GameState.IN_GAME);
        }
        else //if we opened the menu
            GameStateManager.SetGameState(GameState.MENU);

        SoundManager.instance.PlaySound(openInventory);
        noteInventoryOpen = !noteInventoryOpen;
        noteManager.transform.parent.gameObject.SetActive(noteInventoryOpen);
    }

    public void ForceToggleNoteInventoryWindow(bool openOverride)
    {
        if (openOverride) //if it was open, reset UI-related stuff and close
        {
            noteManager.CleanPreviousNoteUI();
            noteManager.ScrollEntriesToLast();
        }

        //GameStateManager.UpdateGameState(openOverride ? GameState.MENU : GameState.IN_GAME);
        SoundManager.instance.PlaySound(openInventory);
        noteInventoryOpen = openOverride;
        noteManager.transform.parent.gameObject.SetActive(openOverride);
    }

    public void OpenNoteInventoryWindowOnUnlock(int unlockedNoteID)
    {
        noteManager.CleanPreviousNoteUI();
        noteManager.ScrollEntries(unlockedNoteID);
        GameStateManager.SetGameState(GameState.MENU);
        noteInventoryOpen = true;
        noteManager.transform.parent.gameObject.SetActive(true);
    }

    void ToggleNotesDescriptionBox()
    {
        noteManager.descriptionUIText.enabled = !noteManager.descriptionUIText.enabled;
    }
    #endregion

    #region ItemInventory methods
    void ToggleItemInventoryWindow()
    {
        itemInventoryOpen = !itemInventoryOpen;

        if (itemInventoryOpen) //if it means that we opened it, then set up item inventory
        {
            itemManager.CleanPreviousUI();
            itemManager.gameObject.SetActive(itemInventoryOpen);
            itemManager.UpdateUI();
            GameStateManager.SetGameState(GameState.MENU);
            SoundManager.instance.PlaySound(openInventory);
        }

        else //if not, clean the UI
        {
            itemManager.CleanPreviousUI();
            itemManager.gameObject.SetActive(itemInventoryOpen);
            GameStateManager.SetGameState(GameState.IN_GAME);
            SoundManager.instance.PlaySound(openInventory);
        }
    }

    public void ToggleItemInventoryWindow(bool openOverride)
    {
        itemInventoryOpen = openOverride;

        if (itemInventoryOpen) //if we opened it, then set up item inventory
        {
            itemManager.gameObject.SetActive(itemInventoryOpen);
            itemManager.UpdateUI();
            SoundManager.instance.PlaySound(openInventory);
        }

        else //if not, clean the UI
        {
            itemManager.CleanPreviousUI();
            itemManager.gameObject.SetActive(itemInventoryOpen);
            SoundManager.instance.PlaySound(openInventory);
        }

        GameStateManager.SetGameState(openOverride ? GameState.MENU : GameState.IN_GAME);
    }
    #endregion
}
