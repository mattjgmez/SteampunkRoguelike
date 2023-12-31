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
            DamageOnTouch damageOnTouch = (weaponHandler.CurrentWeapon as MeleeWeapon).DamageOnTouch;

            if (damageOnTouch == null) { continue; }

            damageOnTouch.DamageCaused = Mathf.RoundToInt(IsMultiply ? damageOnTouch.DamageCaused * _totalBonus : damageOnTouch.DamageCaused + _totalBonus);
        }
    }

    public override void UnapplyUpgrade(Character character)
    {
        if (_totalBonus <= 0 && IsMultiply) { return; }

        InitializeCharacterWeaponList(character);

        foreach (CharacterWeaponHandler weaponHandler in _characterWeapons)
        {
            DamageOnTouch damageOnTouch = (weaponHandler.CurrentWeapon as MeleeWeapon).DamageOnTouch;
            
            if (damageOnTouch == null) { continue; }

            damageOnTouch.DamageCaused = damageOnTouch.BaseDamageCaused;
        }
    }

    protected virtual void InitializeCharacterWeaponList(Character character)
    {
        _characterWeapons = new List<CharacterWeaponHandler>();

        foreach (CharacterAbility ability in character.CharacterAbilities)
        {
            if (ability.GetType() == typeof(CharacterWeaponHandler))
            _characterWeapons.Add(ability as CharacterWeaponHandler);
        }
    }
}
