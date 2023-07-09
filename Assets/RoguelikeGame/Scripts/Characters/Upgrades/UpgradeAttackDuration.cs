using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeAttackDuration : Upgrade
{
    protected List<CharacterWeaponHandler> _characterWeapons;

    public override void ApplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }
        InitializeCharacterWeaponList(character);

        foreach (CharacterWeaponHandler weaponHandler in _characterWeapons)
        {
            Weapon weapon = weaponHandler.CurrentWeapon;

            if (weapon == null || weapon is not MeleeWeapon) { continue; }

            float newDuration = (weapon as MeleeWeapon).ActiveDuration;

            newDuration = IsMultiply ? newDuration * _totalBonus : newDuration + _totalBonus;

            (weapon as MeleeWeapon).ActiveDuration = newDuration;
        }
    }

    public override void UnapplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }
        InitializeCharacterWeaponList(character);

        foreach (CharacterWeaponHandler weaponHandler in _characterWeapons)
        {
            Weapon weapon = weaponHandler.CurrentWeapon;

            if (weapon == null || weapon is not MeleeWeapon) { continue; }

            float newDuration = (weapon as MeleeWeapon).ActiveDuration;

            newDuration = IsMultiply ? newDuration / _totalBonus : newDuration - _totalBonus;

            (weapon as MeleeWeapon).ActiveDuration = newDuration;
        }
    }

    protected virtual void InitializeCharacterWeaponList(Character character)
    {
        _characterWeapons = new List<CharacterWeaponHandler>();

        foreach (CharacterWeaponHandler weaponHandler in character.CharacterAbilities)
        {
            _characterWeapons.Add(weaponHandler);
        }
    }
}
