using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : Item
{
    public Vector3 offset;
    public Vector3 rotation;

    public string[] mainAnimation;
    public string[] secondaryAnimation;

    public int durability;
    public int attack;
    public int speed;

    public bool canBeDropped = true;

    public override void Use(Actor user)
    {
        base.Use(user);
        user.EquipWeapon(this);
    }

    public virtual void MainAttack(Actor actor, int attackNumber)
    {
        if (attackNumber >= mainAnimation.Length)
            actor.AnimateAttack(mainAnimation[mainAnimation.Length - 1]);
        else
            actor.AnimateAttack(mainAnimation[attackNumber]);
    }

    public virtual void SecondaryAttack(Actor actor, int attackNumber)
    {
        if (attackNumber >= secondaryAnimation.Length)
            actor.AnimateAttack(secondaryAnimation[secondaryAnimation.Length - 1]);
        else
            actor.AnimateAttack(secondaryAnimation[attackNumber]);
    }
}
