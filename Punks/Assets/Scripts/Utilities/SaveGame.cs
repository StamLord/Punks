using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame
{
    public static void Save()
    {
        if (CharacterManager.instance)
            CharacterManager.instance.SaveAllCharacters();
    }
}
