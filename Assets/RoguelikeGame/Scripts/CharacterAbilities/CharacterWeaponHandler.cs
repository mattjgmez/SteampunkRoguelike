using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CharacterWeaponHandler : CharacterAbility
{
    #region VARIABLES

    [Header("Input")]
    public bool ContinuousPress = false;
    public bool GettingHitInterruptsAttack = false;

    [Header("Buffering")]
    public bool BufferInput;
    public bool NewInputExtendsBuffer;
    public float MaxBufferDuration = .25f;

    /// the position from which projectiles will be spawned (can be safely left empty)
    public Transform ProjectileSpawn;
    public Weapon CurrentWeapon;

    protected float _fireTimer = 0f;
    protected ProjectileWeapon _projectileWeapon;
    protected float _bufferTimer = 0f;
    protected bool _buffering = false;

    #endregion

    protected override void Initialization()
    {
        base.Initialization();
        Setup();
    }

    /// <summary>
    /// Used for extended flexibility.
    /// </summary>
    public virtual void Setup()
    {
        _character = GetComponent<Character>();

        CurrentWeapon.SetOwner(_character, this);

        _projectileWeapon = CurrentWeapon.gameObject.GetComponent<ProjectileWeapon>();
        if (_projectileWeapon != null)
        {
            _projectileWeapon.SetProjectileSpawnTransform(ProjectileSpawn);
        }
        // we turn off the gun's emitters.
        CurrentWeapon.Initialization();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleBuffer();
    }

    protected override void HandleInput()
    {
        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            return;
        }

        if (_inputManager.ShootButton.State.CurrentState == JP_Input.ButtonStates.ButtonDown)
        {
            //Debug.Log($"{this.GetType()}.HandleInput: Got button down, ShootStart called.", gameObject);
            ShootStart();
        }

        if (_inputManager.ShootButton.State.CurrentState == JP_Input.ButtonStates.ButtonUp)
        {
            Debug.Log($"{this.GetType()}.HandleInput: Got button up, ShootStop called.", gameObject);
            ShootStop();
        }

        if (CurrentWeapon != null)
        {
            if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses
            && _inputManager.ShootButton.State.CurrentState == JP_Input.ButtonStates.Off)
            {
                CurrentWeapon.WeaponInputStop();
            }
        }
    }

    /// <summary>
    /// Triggers an attack if the weapon is idle and an input has been buffered
    /// </summary>
    protected virtual void HandleBuffer()
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        // if we are currently buffering an input and if the weapon is now idle
        if (_buffering && CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle)
        {
            // and if our buffer is still valid, we trigger an attack
            if (Time.time < _bufferTimer)
            {
                ShootStart();
            }
            else
            {
                _buffering = false;
            }
        }
    }

    /// <summary>
    /// Causes the character to start shooting
    /// </summary>
    public virtual void ShootStart()
    {
        // if the Shoot action is enabled in the permissions, we continue, if not we do nothing.  If the player is dead we do nothing.
        if ((CurrentWeapon == null)
        || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
        {
            return;
        }

        //  if we've decided to buffer input, and if the weapon is in use right now
        if (BufferInput && (CurrentWeapon.WeaponState.CurrentState != Weapon.WeaponStates.WeaponIdle))
        {
            // if we're not already buffering, or if each new input extends the buffer, we turn our buffering state to true
            ExtendBuffer();
        }

        CurrentWeapon.WeaponInputStart();
    }

    protected virtual void ExtendBuffer()
    {
        if (!_buffering || NewInputExtendsBuffer)
        {
            _buffering = true;
            _bufferTimer = Time.time + MaxBufferDuration;
        }
    }

    /// <summary>
    /// Causes the character to stop shooting
    /// </summary>
    public virtual void ShootStop()
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponIdle)
        {
            return;
        }

        if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReload)
        || (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStart)
        || (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponReloadStop))
        {
            return;
        }

        if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBeforeUse) 
        && (!CurrentWeapon.DelayBeforeUseReleaseInterruption))
        {
            return;
        }

        if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses) 
        && (!CurrentWeapon.TimeBetweenUsesReleaseInterruption))
        {
            return;
        }

        if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponUse)
        {
            return;
        }

        CurrentWeapon.TurnWeaponOff();
    }

    protected override void OnHit()
    {
        base.OnHit();
        if (GettingHitInterruptsAttack && (CurrentWeapon != null))
        {
            CurrentWeapon.Interrupt();
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        ShootStop();
    }

    protected override void OnRespawn()
    {
        base.OnRespawn();
        Setup();
    }
}
