using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMovespeed : Upgrade
{
    protected float _initialMovementSpeedMultiplier;

    public override void ApplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }
        CharacterMovement movement = character.GetComponent<CharacterMovement>();
        _initialMovementSpeedMultiplier = movement.MovementSpeedMultiplier;

        movement.MovementSpeedMultiplier = IsMultiply ? movement.MovementSpeedMultiplier * _totalBonus : movement.MovementSpeedMultiplier + _totalBonus;
    }

    public override void UnapplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }
        CharacterMovement movement = character.GetComponent<CharacterMovement>();
        _initialMovementSpeedMultiplier = movement.MovementSpeedMultiplier;

        movement.MovementSpeedMultiplier = IsMultiply ? movement.MovementSpeedMultiplier / _totalBonus : movement.MovementSpeedMultiplier - _totalBonus;
    }
}
