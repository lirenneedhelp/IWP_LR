using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Potion : Item
{
    public abstract override void Use();

    public GameObject potionEffectTrail;

}
