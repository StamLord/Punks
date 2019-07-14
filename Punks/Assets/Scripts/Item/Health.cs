using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="HealthItem", menuName = "HealthItem")]
public class Health : Item
{
    public int healthAmount = 50;

    public override void Use(Actor user)
    {
        base.Use(user);
        user.UpdateHealth(healthAmount);
    }
}
