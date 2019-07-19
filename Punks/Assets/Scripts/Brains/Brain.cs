using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Brain : MonoBehaviour
{
    protected Actor actor;
    [SerializeField] protected Schedule schedule;
    [SerializeField] protected float intractionRadius = 2f;
    protected List<GameObject> nearbyInteractableObjects = new List<GameObject>();
    protected IInteractable nearestInteractable;
    protected GameObject nearestInteractableObject;
    protected IInteractable self; //For ignoring

    private bool _inInteraction;
    public bool inInteraction { get { return _inInteraction; } }

    protected virtual void Start()
    {
        actor = GetComponent<Actor>();
        actor.OnDamagedEvent += OnDamaged;
        actor.OnAttackingEvent += OnAttacking;
        self = actor.GetComponent<IInteractable>();
    }

    protected void FindAllInteractables()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, intractionRadius);
        nearbyInteractableObjects.Clear();

        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            IInteractable interactable = nearbyObjects[i].GetComponent<IInteractable>();

            if (interactable != null && interactable != self)
                nearbyInteractableObjects.Add(nearbyObjects[i].gameObject);
        }
    }

    protected void FindNearestInteractble()
    {
        float distance = Mathf.Infinity;
        nearestInteractableObject = null;
        nearestInteractable = null;

        for (int i = 0; i < nearbyInteractableObjects.Count; i++)
        {
            float currentDistance = Vector3.Distance(transform.position, nearbyInteractableObjects[i].transform.position);
            if (currentDistance < distance)
            {
                nearestInteractableObject = nearbyInteractableObjects[i];
                nearestInteractable = nearbyInteractableObjects[i].GetComponent<IInteractable>();
                distance = currentDistance;
            }
        }
    }

    protected List<Brain> FindGangMembers(float radius)
    {
        if (string.IsNullOrEmpty(actor.GetActorData().gang))
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

    public bool InGang(string gang)
    {
        //Check if exists, then check if is equal
        if (GangManager.instance.GetGang(gang) && 
            string.IsNullOrEmpty(actor.GetActorData().gang) == false &&
            actor.GetActorData().gang == gang)
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

    public void SetRoutine(Schedule newRoutine)
    {
        schedule = newRoutine;
    }

    public virtual void StartInteraction()
    {
         _inInteraction = true;
    }

    public virtual void EndInteraction()
    {
        _inInteraction = false;
    }
}
