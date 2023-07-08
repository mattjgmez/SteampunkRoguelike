using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionAimWeaponAtMovement : AIAction
{
    protected TopDownController _controller;
    protected CharacterWeaponHandler _characterWeaponHandler;
    protected WeaponAim _weaponAim;
    protected AIActionShoot _aiActionShoot;
    protected Vector3 _weaponAimDirection;

    protected override void Initialization()
    {
        _characterWeaponHandler = GetComponent<CharacterWeaponHandler>();
        _aiActionShoot = GetComponent<AIActionShoot>();
        _controller = GetComponent<TopDownController>();
    }

    public override void PerformAction()
    {
        if (!Shooting())
        {
            _weaponAimDirection = _controller.CurrentDirection;
            if (_weaponAim == null)
            {
                GrabWeaponAim();
            }
            if (_weaponAim == null)
            {
                return;
            }
            _weaponAim.SetCurrentAim(_weaponAimDirection);
        }
    }

    protected virtual bool Shooting()
    {
        if (_aiActionShoot != null)
        {
            return _aiActionShoot.ActionInProgress;
        }
        return false;
    }

    protected virtual void GrabWeaponAim()
    {
        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            _weaponAim = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<WeaponAim>();
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        GrabWeaponAim();
    }
}
