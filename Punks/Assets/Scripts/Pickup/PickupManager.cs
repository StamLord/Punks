using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pickupPrefab;
    public static PickupManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of PickupManager exists!");
    }

    public void CreatePickup(Vector3 worldPosition, Item item)
    {
        GameObject go = Instantiate(pickupPrefab, worldPosition, Quaternion.identity) as GameObject;
        go.GetComponent<Pickup>().InitializePickup(item);
    }
}
