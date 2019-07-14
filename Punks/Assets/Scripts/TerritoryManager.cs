using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryManager : MonoBehaviour
{
    [SerializeField]
    private List<Territory> territories = new List<Territory>();

    public static TerritoryManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of TerritoryManager exists!");
    }

    private void Start()
    {
        territories = new List<Territory>(GetComponentsInChildren<Territory>());
    }

    void Update()
    {
        
    }

    public Territory FindTerritory(Vector3 position)
    {
        for (int i = 0; i < territories.Count; i++)
        {
            if (territories[i].InTerritory(new Vector2(position.x, position.z)))
                return territories[i];
        }

        return null;
    }

    
}
