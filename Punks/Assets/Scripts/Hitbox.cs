using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private Actor actor;

    void Start()
    {
        if (actor == null)
            Debug.LogWarning("Actor not set for hitbox::" + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root)
            return;

        IDamagable damagable = other.transform.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.Damage(actor.GetStats.attack, actor);
            actor.Attacking(other.GetComponent<Actor>());
        }

        Rigidbody rigidbody = other.transform.GetComponent<Rigidbody>();

        if (rigidbody)
        {
            rigidbody.AddForceAtPosition(((other.transform.position - transform.position).normalized + Vector3.up / 2) * 10, 
                other.ClosestPoint(transform.position),
                ForceMode.Impulse);
        }
    }
}
