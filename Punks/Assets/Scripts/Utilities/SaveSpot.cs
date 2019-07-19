using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSpot : MonoBehaviour, IInteractable
{
    public bool Interact(Actor actor)
    {
        SaveGame.Save();
        return true;
    }

    public string DisplayText()
    {
        return "Save";
    }

}
