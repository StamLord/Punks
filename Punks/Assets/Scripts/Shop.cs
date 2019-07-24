using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private bool buyFromPlayer;

    [SerializeField] Item[] forSale;
    [SerializeField] int[] prices;

    Actor buyer;

    public bool Interact(Brain brain)
    {
        if (buyer != null)
            return false;

        buyer = brain.actor;
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
