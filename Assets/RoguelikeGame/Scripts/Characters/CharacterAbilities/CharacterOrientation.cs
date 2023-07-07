using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CharacterOrientation : CharacterAbility
{
    /// the possible rotation modes
    public enum RotationModes { None, MovementDirection, WeaponDirection, Both }
    /// the possible rotation speeds
    public enum RotationSpeeds { Instant, Smooth, SmoothAbsolute }

    [Header("Rotation Mode")]
    /// whether the character should face movement direction, weapon direction, or both, or none
    public RotationModes RotationMode = RotationModes.None;
    /// if this is false, no rotation will occur
    public bool CharacterRotationAuthorized = true;

    [Header("Movement Direction")]
    /// If this is true, we'll rotate our model towards the direction
    public bool ShouldRotateToFaceMovementDirection = true;
    /// the current rotation mode
    public RotationSpeeds MovementRotationSpeed = RotationSpeeds.Instant;
    /// the object we want to rotate towards direction. If left empty, we'll use the Character's model
    public GameObject MovementRotatingModel;
    /// the speed at which to rotate towards direction (smooth and absolute only)
    public float RotateToFaceMovementDirectionSpeed = 10f;
    /// the threshold after which we start rotating (absolute mode only)
    public float AbsoluteThresholdMovement = 0.5f;
    /// the direction of the model
    public Vector3 ModelDirection;
    /// the direction of the model in angle values
    public Vector3 ModelAngles;

    [Header("Weapon Direction")]
    /// If this is true, we'll rotate our model towards the weapon's direction
    public bool ShouldRotateToFaceWeaponDirection = true;
    /// the current rotation mode
    public RotationSpeeds WeaponRotationSpeed = RotationSpeeds.Instant;
    /// the object we want to rotate towards direction. If left empty, we'll use the Character's model
    public GameObject WeaponRotatingModel;
    /// the speed at which to rotate towards direction (smooth and absolute only)
    public float RotateToFaceWeaponDirectionSpeed = 10f;
    /// the threshold after which we start rotating (absolute mode only)
    public float AbsoluteThresholdWeapon = 0.5f;

    [Header("Animation")]
    /// the speed at which the instant rotation animation parameter float resets to 0
    public float RotationSpeedResetSpeed = 2f;

    [Header("Forced Rotation")]
    /// whether the character is being applied a forced rotation
    public bool ForcedRotation = false;
    /// the forced rotation applied by an external script
    public Vector3 ForcedRotationDirection;

    protected CharacterWeaponHandler _characterWeaponHandler;
    protected Vector3 _rotationDirection;
    protected Vector3 _lastMovement = Vector3.zero;

    protected Quaternion _tmpRotation;
    protected Quaternion _newMovementQuaternion;
    protected Quaternion _newWeaponQuaternion;
    protected bool _shouldRotateTowardsWeapon;
    protected float _rotationSpeed;
    protected float _modelAnglesYLastFrame;
    protected Vector3 _currentDirection;

    protected override void Initialization()
    {
        base.Initialization();
        if (MovementRotatingModel == null)
        {
            MovementRotatingModel = _model;
        }
        _characterWeaponHandler = this.gameObject.GetComponent<CharacterWeaponHandler>();
        if (WeaponRotatingModel == null)
        {
            WeaponRotatingModel = _model;
        }
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        //if (GameManager.Instance.Paused) { return; }

        if (CharacterRotationAuthorized)
        {
            RotateToFaceMovementDirection();
            RotateToFaceWeaponDirection();
            RotateModel();
        }
    }

    protected virtual void RotateToFaceMovementDirection()
    {
        // if we're not supposed to face our direction, we do nothing and exit
        if (!ShouldRotateToFaceMovementDirection) { return; }
        if ((RotationMode != RotationModes.MovementDirection) && (RotationMode != RotationModes.Both)) { return; }

        _currentDirection = ForcedRotation ? ForcedRotationDirection : _controller.CurrentDirection;

        switch (MovementRotationSpeed)
        {
            // if the rotation mode is instant, we simply rotate to face our direction
            case RotationSpeeds.Instant:
                if (_currentDirection != Vector3.zero)
                {
                    _newMovementQuaternion = Quaternion.LookRotation(_currentDirection);
                }
                break;
            // if the rotation mode is smooth, we lerp towards our direction
            case RotationSpeeds.Smooth:
                if (_currentDirection != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_currentDirection);
                    _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceMovementDirectionSpeed);
                }
                break;
            // if the rotation mode is smooth, we lerp towards our direction even if the input has been released
            case RotationSpeeds.SmoothAbsolute:
                if (_currentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
                {
                    _lastMovement = _currentDirection;
                }
                if (_lastMovement != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_lastMovement);
                    _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceMovementDirectionSpeed);
                }
                break;
        }

        ModelDirection = MovementRotatingModel.transform.forward.normalized;
        ModelAngles = MovementRotatingModel.transform.eulerAngles;
    }

    protected virtual void RotateToFaceWeaponDirection()
    {
        _newWeaponQuaternion = Quaternion.identity;
        _shouldRotateTowardsWeapon = false;

        // if we're not supposed to face our direction, we do nothing and exit
        if (!ShouldRotateToFaceWeaponDirection) { return; }
        if ((RotationMode != RotationModes.WeaponDirection) && (RotationMode != RotationModes.Both)) { return; }
        if (_characterWeaponHandler == null) { return; }
        if (_characterWeaponHandler.WeaponAimComponent == null) { return; }

        _shouldRotateTowardsWeapon = true;

        _rotationDirection = _characterWeaponHandler.WeaponAimComponent.CurrentAim.normalized;

        switch (WeaponRotationSpeed)
        {
            // if the rotation mode is instant, we simply rotate to face our direction
            case RotationSpeeds.Instant:
                if (_rotationDirection != Vector3.zero)
                {
                    _newWeaponQuaternion = Quaternion.LookRotation(_rotationDirection);
                }
                break;

            // if the rotation mode is smooth, we lerp towards our direction
            case RotationSpeeds.Smooth:
                if (_rotationDirection != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_rotationDirection);
                    _newWeaponQuaternion = Quaternion.Slerp(WeaponRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceWeaponDirectionSpeed);
                }
                break;

            // if the rotation mode is smooth, we lerp towards our direction even if the input has been released
            case RotationSpeeds.SmoothAbsolute:
                if (_rotationDirection.normalized.magnitude >= AbsoluteThresholdWeapon)
                {
                    _lastMovement = _rotationDirection;
                }
                if (_lastMovement != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_lastMovement);
                    _newWeaponQuaternion = Quaternion.Slerp(WeaponRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceWeaponDirectionSpeed);
                }
                break;
        }
    }

    protected virtual void RotateModel()
    {
        MovementRotatingModel.transform.rotation = _newMovementQuaternion;

        if (_shouldRotateTowardsWeapon)
        {
            WeaponRotatingModel.transform.rotation = _newWeaponQuaternion;
        }
    }

    protected virtual void LateUpdate()
    {
        ComputeRelativeSpeeds();
    }

    protected virtual void ComputeRelativeSpeeds()
    {
        // RotationSpeed
        if (Mathf.Abs(_modelAnglesYLastFrame - ModelAngles.y) > 1f)
        {
            _rotationSpeed = Mathf.Abs(_modelAnglesYLastFrame - ModelAngles.y);
        }
        else
        {
            _rotationSpeed -= Time.time * RotationSpeedResetSpeed;
        }
        if (_rotationSpeed <= 0f)
        {
            _rotationSpeed = 0f;
        }

        _modelAnglesYLastFrame = ModelAngles.y;
    }
}
