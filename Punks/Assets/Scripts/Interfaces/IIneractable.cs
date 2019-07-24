using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool Interact(Brain brain);

    string DisplayText();
}
