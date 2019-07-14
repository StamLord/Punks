using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    bool Damage(int damage, Actor attacker);
}
