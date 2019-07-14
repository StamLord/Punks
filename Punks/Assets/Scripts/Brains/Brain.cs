using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Brain : MonoBehaviour
{
    protected Actor actor;
    [SerializeField]
    protected float intractionRadius = 2f;
    protected IInteractable nearestInteractable;
    protected IInteractable self; //For ignoring

    protected virtual void Start()
    {
        actor = GetComponent<Actor>();
        actor.OnDamagedEvent += OnDamaged;
        actor.OnAttackingEvent += OnAttacking;
        self = actor.GetComponent<IInteractable>();
    }

    protected void FindInteractables()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, intractionRadius);
        List<GameObject> nearbyInteractables = new List<GameObject>();

        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            IInteractable interactable = nearbyObjects[i].GetComponent<IInteractable>();

            if (interactable != null && interactable != self)
                nearbyInteractables.Add(nearbyObjects[i].gameObject);
        }

        float distance = Mathf.Infinity;
        for (int i = 0; i < nearbyInteractables.Count; i++)
        {
            float currentDistance = Vector3.Distance(transform.position, nearbyInteractables[i].transform.position);
            if (currentDistance < distance)
            {
                nearestInteractable = nearbyInteractables[i].GetComponent<IInteractable>();
                distance = currentDistance;
            }
        }
    }

    protected List<Brain> FindGangMembers(float radius)
    {
        if (actor.GetActorData().gang == null)
            return null;

        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, radius);
        List<Brain> members = new List<Brain>();

        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            Brain brain = nearbyObjects[i].GetComponent<Brain>();
            if (brain && brain.InGang(actor.GetActorData().gang))
                members.Add(brain);

        }

        return members;
    }

    public bool InGang(Gang gang)
    {
        if (actor.GetActorData().gang == gang)
            return true;
        else
            return false;
    }

    public virtual void AddEnemy(Actor enemy)
    {

    }

    protected virtual void OnDamaged(Actor attacker)
    {

    }

    protected virtual void OnAttacking(Actor enemy)
    {

    }
}
