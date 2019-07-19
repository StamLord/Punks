using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string _displayText;
    [SerializeField] private Item _item;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Transform modelParent;

    public bool Interact(Actor actor)
    {
        Debug.Log(actor.name + "::Tried to pick up::" + gameObject.name);
        return PickupItem(actor);
    }

    public string DisplayText()
    {
        return _displayText;
    }

    bool PickupItem(Actor actor)
    {
        if(_item != null)
            _item.Use(actor);
        Destroy(gameObject);
        return true;
    }

    public void InitializePickup(Item item)
    {
        _item = item;
        Instantiate(item.model, modelParent);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
    }
}
