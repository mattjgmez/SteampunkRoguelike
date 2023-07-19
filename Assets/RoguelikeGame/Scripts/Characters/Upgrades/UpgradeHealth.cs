using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeHealth : Upgrade
{
    public override void ApplyUpgrade(Character character)
    {
        Debug.Log($"{this.GetType()}.ApplyUpgrade: TotalBonus is {_totalBonus}. Applying Upgrade", character.gameObject);

        if (_totalBonus <= 0 && IsMultiply) { return; }

        Health health = character.GetComponent<Health>();

        health.MaxHealth = Mathf.RoundToInt(IsMultiply ? health.MaxHealth * _totalBonus : health.MaxHealth + _totalBonus);
        health.CurrentHealth = Mathf.RoundToInt(IsMultiply ? health.CurrentHealth * _totalBonus : health.CurrentHealth + _totalBonus);
    }

    public override void UnapplyUpgrade(Character character)
    {
        Debug.Log($"{this.GetType()}.UnapplyUpgrade: TotalBonus is {_totalBonus}. Unapplying Upgrade.", character.gameObject);

        if (_totalBonus <= 0 && IsMultiply) { return; }

        Health health = character.GetComponent<Health>();

        health.MaxHealth = health.InitialHealth;
        health.CurrentHealth = health.InitialHealth;
    }
}
