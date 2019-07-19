using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private bool buyFromPlayer;

    [SerializeField] Item[] forSale;
    [SerializeField] int[] prices;

    Actor buyer;

    public bool Interact(Actor actor)
    {
        if (buyer != null)
            return false;

        buyer = actor;
        OpenShopWindow();
        return true;
    }

    public string DisplayText()
    {
        return "Shop";
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OpenShopWindow()
    {

    }

    public bool Buy(int index)
    {
        if (buyer.GetActorData().money < prices[index])
            return false;
        else
            return true;
    }
}
