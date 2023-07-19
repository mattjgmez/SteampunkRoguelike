using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMovespeed : Upgrade
{
    public override void ApplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }
        CharacterMovement movement = character.GetComponent<CharacterMovement>();

        movement.MovementSpeedMultiplier = IsMultiply ? movement.MovementSpeedMultiplier * _totalBonus : movement.MovementSpeedMultiplier + _totalBonus;
    }

    public override void UnapplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }
        CharacterMovement movement = character.GetComponent<CharacterMovement>();

        movement.MovementSpeedMultiplier = 1f;
    }
}
