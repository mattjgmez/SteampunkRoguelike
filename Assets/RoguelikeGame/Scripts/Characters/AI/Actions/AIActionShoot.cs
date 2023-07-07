using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionShoot : AIAction
{
    public bool FaceTarget = true;
    public bool AimAtTarget = false;
    public Vector3 ShootOffset;

    protected CharacterOrientation _orientation;
    protected Character _character;
    protected CharacterWeaponHandler _characterWeaponHandler;
    protected WeaponAim _weaponAim;
    protected ProjectileWeapon _projectileWeapon;
    protected Vector3 _weaponAimDirection;
    protected int _numberOfShots = 0;
    protected bool _shooting = false;

    protected override void Initialization()
    {
        _character = GetComponent<Character>();
        _orientation = GetComponent<CharacterOrientation>();
        _characterWeaponHandler = this.gameObject.GetComponent<CharacterWeaponHandler>();
    }

    public override void PerformAction()
    {
        MakeChangesToTheWeapon();
        TestAimAtTarget();
        Shoot();
    }

    protected virtual void MakeChangesToTheWeapon()
    {
        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            _characterWeaponHandler.CurrentWeapon.TimeBetweenUsesReleaseInterruption = true;
        }
    }

    protected virtual void TestAimAtTarget()
    {
        if (!AimAtTarget || (_brain.Target == null))
        {
            return;
        }

        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            if (_weaponAim == null)
            {
                _weaponAim = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<WeaponAim>();
            }

            if (_weaponAim != null)
            {
                if (_projectileWeapon != null)
                {
                    _projectileWeapon.DetermineSpawnPosition();
                    _weaponAimDirection = _brain.Target.position + ShootOffset - (_character.transform.position);
                }
                else
                {
                    _weaponAimDirection = _brain.Target.position + ShootOffset - _character.transform.position;
                }
            }
        }
    }

    protected virtual void Shoot()
    {
        if (_numberOfShots < 1)
        {
            _characterWeaponHandler.ShootStart();
            _numberOfShots++;
        }
    }

    protected virtual void Update()
    {
        if (_characterWeaponHandler.CurrentWeapon != null)
        {
            if (_weaponAim != null)
            {
                if (_shooting)
                {
                    _weaponAim.SetCurrentAim(_weaponAimDirection);
                }
            }
        }
    }

    /// <summary>
    /// When entering the state we reset our shoot counter and grab our weapon
    /// </summary>
    public override void OnEnterState()
    {
        base.OnEnterState();
        _numberOfShots = 0;
        _shooting = true;
        _weaponAim = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<WeaponAim>();
        _projectileWeapon = _characterWeaponHandler.CurrentWeapon.gameObject.GetComponent<ProjectileWeapon>();
    }

    /// <summary>
    /// When exiting the state we make sure we're not shooting anymore
    /// </summary>
    public override void OnExitState()
    {
        base.OnExitState();
        _characterWeaponHandler.ShootStop();
        _shooting = false;
    }
}
