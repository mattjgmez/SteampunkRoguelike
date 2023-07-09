using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDamage : Upgrade
{
    protected List<CharacterWeaponHandler> _characterWeapons;

    public override void ApplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }

        InitializeCharacterWeaponList(character);

        foreach (CharacterWeaponHandler weaponHandler in _characterWeapons)
        {
            DamageOnTouch damageOnTouch = weaponHandler.CurrentWeapon.GetComponent<DamageOnTouch>();

            if (damageOnTouch == null) { continue; }

            damageOnTouch.DamageCaused = IsMultiply ? damageOnTouch.DamageCaused * (int)_totalBonus : damageOnTouch.DamageCaused + (int)_totalBonus;
        }
    }

    public override void UnapplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }

        InitializeCharacterWeaponList(character);

        foreach (CharacterWeaponHandler weaponHandler in _characterWeapons)
        {
            DamageOnTouch damageOnTouch = weaponHandler.CurrentWeapon.GetComponent<DamageOnTouch>();
            
            if (damageOnTouch == null) { continue; }

            damageOnTouch.DamageCaused = IsMultiply ? damageOnTouch.DamageCaused / (int)_totalBonus : damageOnTouch.DamageCaused - (int)_totalBonus;
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
