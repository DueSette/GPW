﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelButton : MonoBehaviour, IInteractable, ITextPrompt
{
    void IInteractable.InteractWith()
    {
        LevelManager.onEventLevelEnd();
    }

    string ITextPrompt.PromptText()
    {
        return "Time to move to the next level...";
    }
}
