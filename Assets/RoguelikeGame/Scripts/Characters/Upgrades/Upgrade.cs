using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    [Header("Info")]
    public string Name = "";
    public string Description = "";
    public Sprite Sprite;

    public abstract void ProcessUpgrade();
    public int AmountActive = 0;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization() { }
}
