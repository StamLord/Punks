using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    new public string name;
    public GameObject model;

    public virtual void Use(Actor user)
    {
        Debug.Log(user.name + " :: Used :: " + name);
    }

}
