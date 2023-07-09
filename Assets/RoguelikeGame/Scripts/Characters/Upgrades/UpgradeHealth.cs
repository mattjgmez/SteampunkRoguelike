using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHealth : Upgrade
{
    public override void ApplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }

        Health health = character.GetComponent<Health>();

        health.MaxHealth = IsMultiply ? health.MaxHealth * (int)_totalBonus : health.MaxHealth + (int)_totalBonus;
        health.InitialHealth = IsMultiply ? health.InitialHealth * (int)_totalBonus : health.InitialHealth + (int)_totalBonus;
    }

    public override void UnapplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }

        Health health = character.GetComponent<Health>();

        health.MaxHealth = IsMultiply ? health.MaxHealth / (int)_totalBonus : health.MaxHealth - (int)_totalBonus;
        health.InitialHealth = IsMultiply ? health.InitialHealth / (int)_totalBonus : health.InitialHealth - (int)_totalBonus;
    }
}
