using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // hit delegate
    public delegate void OnHitDelegate();
    public OnHitDelegate OnHit;

    // respawn delegate
    public delegate void OnReviveDelegate();
    public OnReviveDelegate OnRevive;

    // death delegate
    public delegate void OnDeathDelegate();
    public OnDeathDelegate OnDeath;
}
