using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponStates
    {
        WeaponIdle,
        WeaponStart,
        WeaponDelayBeforeUse,
        WeaponUse,
        WeaponDelayBetweenUses,
        WeaponStop,
        WeaponReloadStart,
        WeaponReload,
        WeaponReloadStop,
        WeaponInterrupted
    }

    [Header("Use")]
    /// the delay before use, that will be applied for every shot
    public float DelayBeforeUse = 0f;
    /// whether or not the delay before used can be interrupted by releasing the shoot button (if true, releasing the button will cancel the delayed shot)
    public bool DelayBeforeUseReleaseInterruption = true;
    /// the time (in seconds) between two shots		
    public float TimeBetweenUses = 1f;
    /// whether or not the time between uses can be interrupted by releasing the shoot button (if true, releasing the button will cancel the time between uses)
    public bool TimeBetweenUsesReleaseInterruption = true;

    [Header("Magazine")]
    /// whether or not the weapon is magazine based. If it's not, it'll just take its ammo inside a global pool
    public bool MagazineBased = false;
    /// the size of the magazine
    public int MagazineSize = 30;
    /// the time it takes to reload the weapon
    public float ReloadTime = 2f;
    /// the amount of ammo consumed everytime the weapon fires
    public int AmmoConsumedPerShot = 1;
    /// the current amount of ammo loaded inside the weapon
    public int CurrentAmmoLoaded = 0;

    [Header("Recoil")]
    public float RecoilForce = 0f;

    [Header("Settings")]
    /// If this is true, the weapon will initialize itself on start, otherwise it'll have to be init manually, usually by the CharacterHandleWeapon class
    public bool InitializeOnStart = false;
    /// Controls if the weapon can be interrupted or not.
    public bool Interruptable = false;

    /// the weapon's owner
    public Character Owner { get; protected set; }
    /// the weapon's owner's CharacterWeaponHandler component
    public CharacterWeaponHandler CharacterWeaponHandler { get; set; }
    // the weapon's state machine
    public StateMachine<WeaponStates> WeaponState;

    protected float _delayBeforeUseCounter = 0f;
    protected float _delayBetweenUsesCounter = 0f;
    protected float _reloadingCounter = 0f;
    protected bool _triggerReleased = false;
    protected bool _reloading = false;
    protected TopDownController _controller;
    // protected CharacterMovement _characterMovement;

    protected virtual void Start()
    {
        if (InitializeOnStart)
        {
            Initialization();
        }
    }

    public virtual void Initialization()
    {
        WeaponState = new StateMachine<WeaponStates>(gameObject, true);
        WeaponState.ChangeState(WeaponStates.WeaponIdle);
        CurrentAmmoLoaded = MagazineSize;
    }

    public virtual void SetOwner(Character owner, CharacterWeaponHandler weaponHandler)
    {
        Owner = owner;
        if (Owner != null)
        {
            CharacterWeaponHandler = weaponHandler;
            //_characterMovement = Owner.GetComponent<CharacterMovement>();
            _controller = Owner.GetComponent<TopDownController>();
        }
    }

    public virtual void WeaponInputStart()
    {
        if (_reloading) { return; }

        if (WeaponState.CurrentState == WeaponStates.WeaponIdle)
        {
            _triggerReleased = false;
            TurnWeaponOn();
        }
    }

    public virtual void WeaponInputStop()
    {
        if (_reloading)
        {
            return;
        }
        _triggerReleased = true;
    }

    /// <summary>
    /// Handle what happens when the weapon starts
    /// </summary>
    protected virtual void TurnWeaponOn()
    {
        WeaponState.ChangeState(WeaponStates.WeaponStart);
    }

    protected virtual void LateUpdate()
    {
        ProcessWeaponState();
    }

    protected virtual void ProcessWeaponState()
    {
        if (WeaponState == null) { return; } // Placeholder for potential state machine

        switch (WeaponState.CurrentState)
        {
            case WeaponStates.WeaponIdle:
                CaseWeaponIdle();
                break;
            case WeaponStates.WeaponDelayBeforeUse:
                CaseWeaponDelayBeforeUse();
                break;
            case WeaponStates.WeaponStart:
                CaseWeaponStart();
                break;
            case WeaponStates.WeaponUse:
                CaseWeaponUse();
                break;
            case WeaponStates.WeaponDelayBetweenUses:
                CaseWeaponDelayBetweenUses();
                break;
            case WeaponStates.WeaponStop:
                CaseWeaponStop();
                break;

            case WeaponStates.WeaponReloadStart:
                CaseWeaponReloadStart();
                break;
            case WeaponStates.WeaponReload:
                CaseWeaponReload();
                break;
            case WeaponStates.WeaponReloadStop:
                CaseWeaponReloadStop();
                break;

            case WeaponStates.WeaponInterrupted:
                CaseWeaponInterrupted();
                break;
        }
    }

    #region WEAPON STATE MACHINE CASE METHODS

    /// <summary>
    /// If the weapon is idle, we currently do nothing
    /// </summary>
    public virtual void CaseWeaponIdle()
    {
        
    }

    /// <summary>
    /// When the weapon starts we switch to a delay or shoot based on our weapon's settings
    /// </summary>
    public virtual void CaseWeaponStart()
    {
        if (DelayBeforeUse > 0)
        {
            _delayBeforeUseCounter = DelayBeforeUse;
            WeaponState.ChangeState(WeaponStates.WeaponDelayBeforeUse);
        }
        else
        {
            ShootRequest();
        }
    }

    /// <summary>
    /// If we're in delay before use, we wait until our delay is passed and then request a shoot
    /// </summary>
    public virtual void CaseWeaponDelayBeforeUse()
    {
        _delayBeforeUseCounter -= Time.deltaTime;
        if (_delayBeforeUseCounter <= 0) 
        {
            ShootRequest();
        }
    }

    /// <summary>
    /// On weapon use we use our weapon then switch to delay between uses
    /// </summary>
    public virtual void CaseWeaponUse()
    {
        WeaponUse();
        _delayBetweenUsesCounter = TimeBetweenUses;
        WeaponState.ChangeState(WeaponStates.WeaponDelayBetweenUses);
    }

    /// <summary>
    /// When in delay between uses, we either turn our weapon off or make a shoot request
    /// </summary>
    public virtual void CaseWeaponDelayBetweenUses()
    {
        _delayBetweenUsesCounter -= Time.deltaTime;
        if (_delayBetweenUsesCounter <= 0)
        {
            TurnWeaponOff();
        }
    }

    /// <summary>
    /// On weapon stop, we switch back to idle
    /// </summary>
    public virtual void CaseWeaponStop()
    {
        WeaponState.ChangeState(WeaponStates.WeaponIdle);
    }

    /// <summary>
    /// on reload start, we reload the weapon and switch to reload
    /// </summary>
    public virtual void CaseWeaponReloadStart()
    {
        // add method to trigger reload animation
        _reloadingCounter = ReloadTime;
        WeaponState.ChangeState(WeaponStates.WeaponReload);
    }

    /// <summary>
    /// on reload, we reset our movement multiplier, and switch to reload stop once our reload delay has passed
    /// </summary>
    public virtual void CaseWeaponReload()
    {
        _reloadingCounter -= Time.deltaTime;
        if (_reloadingCounter <= 0)
        {
            WeaponState.ChangeState(WeaponStates.WeaponReloadStop);
        }
    }

    /// <summary>
    /// on reload stop, we swtich to idle and load our ammo
    /// </summary>
    public virtual void CaseWeaponReloadStop()
    {
        _reloading = false;
        WeaponState.ChangeState(WeaponStates.WeaponIdle);
        CurrentAmmoLoaded = MagazineSize;
    }

    public virtual void CaseWeaponInterrupted()
    {
        TurnWeaponOff();
        WeaponState.ChangeState(WeaponStates.WeaponIdle);
    }

    /// <summary>
    /// Call this method to interrupt the weapon
    /// </summary>
    public virtual void Interrupt()
    {
        if (Interruptable)
        {
            WeaponState.ChangeState(WeaponStates.WeaponInterrupted);
        }
    }

    #endregion

    public virtual void ShootRequest()
    {
        if (_reloading) return;

        //Debug.Log($"{this.GetType()}.ShootRequest: Shot requested.", gameObject);

        if (MagazineBased)
        {
            if (CurrentAmmoLoaded > 0)
            {
                WeaponState.ChangeState(WeaponStates.WeaponUse);
                CurrentAmmoLoaded -= AmmoConsumedPerShot;
            }
            else
            {
                InitiateReloadWeapon();
            }
        }
        else
        {
            WeaponState.ChangeState(WeaponStates.WeaponUse);
        }
    }

    /// <summary>
    /// When the weapon is used, trigger relevant behavior such as recoil and sound
    /// </summary>
    public virtual void WeaponUse()
    {
        //Debug.Log($"{this.GetType()}.WeaponUse: WeaponUse called.", gameObject);
        // apply recoil
        if (RecoilForce != 0f && _controller != null && Owner != null)
        {
            _controller.Impact(-this.transform.forward, RecoilForce);
        }
        // here is where sfx and vfx would be triggered
    }

    /// <summary>
    /// Turns the weapon off.
    /// </summary>
    public virtual void TurnWeaponOff()
    {
        if ((WeaponState.CurrentState == WeaponStates.WeaponIdle || WeaponState.CurrentState == WeaponStates.WeaponStop))
        {
            return;
        }
        _triggerReleased = true;

        //Debug.Log($"{this.GetType()}.TurnWeaponOff: Attempting to change state to WeaponStop.", gameObject);

        WeaponState.ChangeState(WeaponStates.WeaponStop);
    }

    public virtual void InitiateReloadWeapon()
    {
        // if we're already reloading, we do nothing and exit
        if (_reloading)
        {
            return;
        }
        WeaponState.ChangeState(WeaponStates.WeaponReloadStart);
        _reloading = true;
    }
}
