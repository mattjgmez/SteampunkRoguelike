using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JadePhoenix.Tools;
using System;
using static Cinemachine.CinemachineTargetGroup;

[RequireComponent(typeof(Weapon))]
public class WeaponAim : MonoBehaviour
{
    public enum AimControls { Off, Mouse, Script}

    [Header("Control Mode")]
    public AimControls AimControl = AimControls.Off;

    [Header("Weapon Rotation")]
    public float WeaponRotationSpeed = 1f;
    public float MinimumAngle = -180f;
    public float MaximumAngle = 180f;
    public float MinimumMagnitude = 0.2f;

    [Header("CameraTarget")]
    public bool MoveCameraTargetTowardsReticle = false;
    [Range(0f, 1f)]
    public float CameraTargetOffset = 0.3f;
    public float CameraTargetMaxDistance = 10f;
    public float CameraTargetSpeed = 5f;

    public float CurrentAngleAbsolute { get; protected set; }
    public Quaternion CurrentRotation { get { return transform.rotation; } }
    public Vector3 CurrentAim { get { return _currentAim; } }
    /// the current angle the weapon is aiming at
    public float CurrentAngle { get; protected set; }
    /// the current angle the weapon is aiming at, adjusted to compensate for the current orientation of the character
    public virtual float CurrentAngleRelative
    {
        get
        {
            if (_weapon != null)
            {
                if (_weapon.Owner != null)
                {
                    return CurrentAngle;
                }
            }
            return 0;
        }
    }

    protected Weapon _weapon;
    protected Vector3 _currentAim = Vector3.zero;
    protected Quaternion _lookRotation;
    protected Vector3 _direction;
    protected float _additionalAngle;
    protected Quaternion _initialRotation;
    protected Plane _playerPlane;
    protected Vector3 _reticlePosition;
    protected Vector3 _newCamTargetPosition;
    protected Vector3 _newCamTargetDirection;
    protected Vector2 _inputMovement;
    protected Camera _mainCamera;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _weapon = GetComponent<Weapon>();

        _initialRotation = transform.rotation;
        _playerPlane = new Plane(Vector3.up, Vector3.zero);
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Aims the weapon towards a new point
    /// </summary>
    /// <param name="newAim">New aim.</param>
    public virtual void SetCurrentAim(Vector3 newAim)
    {
        _currentAim = newAim;
    }

    protected virtual void Update()
    {
        GetCurrentAim();
        DetermineWeaponRotation();
        MoveTarget();
        UpdatePlane();
    }

    #region CURRENT AIM METHODS

    protected virtual void GetCurrentAim()
    {
        if (_weapon.Owner == null) { return; }

        if ((_weapon.Owner.LinkedInputManager == null) && (_weapon.Owner.CharacterType == Character.CharacterTypes.Player)) { return; }

        switch (AimControl)
        {
            case AimControls.Off:
                if (_weapon.Owner == null) { return; }
                GetOffAim();
                break;

            case AimControls.Script:
                GetScriptAim();
                break;

            case AimControls.Mouse:
                if (_weapon.Owner == null)
                {
                    return;
                }
                GetMouseAim();
                break;
        }
    }

    protected virtual void GetOffAim()
    {
        _currentAim = Vector3.right;
        _direction = Vector3.right;
    }

    protected virtual void GetScriptAim()
    {
        _direction = -(transform.position - _currentAim);
    }

    protected virtual void GetMouseAim()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
        float distance;
        if (_playerPlane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            _direction = target;
        }

        _reticlePosition = _direction;

        _direction.y = transform.position.y;
        _currentAim = _direction - transform.position;
    }

    #endregion

    /// <summary>
    /// Determines the weapon's rotation
    /// </summary>
    protected virtual void DetermineWeaponRotation()
    {
        if (_currentAim != Vector3.zero)
        {
            if (_direction != Vector3.zero)
            {
                CurrentAngle = Mathf.Atan2(_currentAim.z, _currentAim.x) * Mathf.Rad2Deg;

                // we add our additional angle
                CurrentAngle += _additionalAngle;

                // we clamp the angle to the min/max values set in the inspector

                CurrentAngle = Mathf.Clamp(CurrentAngle, MinimumAngle, MaximumAngle);
                CurrentAngle = -CurrentAngle + 90f;

                _lookRotation = Quaternion.Euler(CurrentAngle * Vector3.up);
                RotateWeapon(_lookRotation);
            }
        }
        else
        {
            CurrentAngle = 0f;
            RotateWeapon(_initialRotation);
        }
    }

    protected virtual void RotateWeapon(Quaternion newRotation)
    {
        /// Placeholder for eventual GameManager
        //if (GameManager.Instance.Paused)
        //{
        //    return;
        //}

        // if the rotation speed is == 0, we have instant rotation
        if (WeaponRotationSpeed == 0)
        {
            transform.rotation = newRotation;
        }
        // otherwise we lerp the rotation
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, WeaponRotationSpeed * Time.deltaTime);
        }
    }

    protected virtual void MoveTarget()
    {
        if (MoveCameraTargetTowardsReticle && (_weapon.Owner != null))
        {
            _newCamTargetPosition = _reticlePosition;
            _newCamTargetDirection = _newCamTargetPosition - this.transform.position;
            if (_newCamTargetDirection.magnitude > CameraTargetMaxDistance)
            {
                _newCamTargetDirection = _newCamTargetDirection.normalized * CameraTargetMaxDistance;
            }
            _newCamTargetPosition = this.transform.position + _newCamTargetDirection;

            //Debug.Log($"{this.GetType()}.MoveTarget: _weapon.Owner: {_weapon.Owner} _newCamTargetPosition: {_newCamTargetPosition} CameraTargetOffset {CameraTargetOffset}.", gameObject);

            _newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position, 
                                    Vector3.Lerp(this.transform.position, _newCamTargetPosition, CameraTargetOffset), Time.deltaTime * CameraTargetSpeed);

            _weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
        }
    }

    protected virtual void UpdatePlane()
    {
        _playerPlane.SetNormalAndPosition(Vector3.up, this.transform.position);
    }

    /// <summary>
    /// On LateUpdate, resets any additional angle
    /// </summary>
    protected virtual void LateUpdate()
    {
        ResetAdditionalAngle();
    }

    /// <summary>
    /// Resets the additional angle
    /// </summary>
    protected virtual void ResetAdditionalAngle()
    {
        _additionalAngle = 0;
    }
}
