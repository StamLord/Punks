using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LocationsManager : MonoBehaviour
{
    [SerializeField] private string[] locationNames;
    [SerializeField] private Transform[] locationPositions;

    private Dictionary<string, Transform> locations = new Dictionary<string, Transform>();

    public static LocationsManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of LoctionsManager exists!");
    }

    private void Start()
    {
        for (int i = 0; i < locationNames.Length; i++)
        {
            if (i < locationPositions.Length)
                locations.Add(locationNames[i], locationPositions[i]);
            else
                Debug.Log("Location names and positions have different numbers");
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < locationNames.Length; i++)
        {
            Gizmos.DrawWireCube(locationPositions[i].position, Vector3.one);
            //Handles.Label(locationPositions[i].position, locationNames[i]);
        }
    }

    public bool LocationExists(string locationName)
    {
        return (locations.ContainsKey(locationName));
    }

    public Transform GetLocation(string locationName)
    {
        return locations[locationName];
    }

}
