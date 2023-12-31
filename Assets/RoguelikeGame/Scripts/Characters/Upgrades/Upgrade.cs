using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    [Header("Info")]
    public string Label = "";
    public string Description = "";
    public Sprite Sprite;
    public float UpgradeImpact = 1;
    public bool IsMultiply = false;

    public abstract void ApplyUpgrade(Character character);
    public abstract void UnapplyUpgrade(Character character);
    public int AmountActive = 0;

    public float _totalBonus;

    protected virtual void Start()
    {
        CalculateBonus();
    }

    public virtual void CalculateBonus() 
    {
        _totalBonus = (UpgradeImpact * AmountActive) + (IsMultiply ? 1 : 0);
    }
}
