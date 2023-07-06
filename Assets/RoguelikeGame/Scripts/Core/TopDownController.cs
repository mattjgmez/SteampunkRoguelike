using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CharacterController))]

public class TopDownController : MonoBehaviour
{
    #region PUBLIC VARIABLES

    public Vector3 Speed;
    public Vector3 Velocity;
    public Vector3 VelocityLastFrame;
    public Vector3 Acceleration;
    public Vector3 CurrentMovement;
    public Vector3 CurrentDirection;
    public float Friction;
    public Vector3 AddedForce;
    
    public Vector3 InputMoveDirection = Vector3.zero;

    public enum UpdateModes { Update, FixedUpdate }

    [Header("Settings")]
    public UpdateModes UpdateMode = UpdateModes.FixedUpdate;

    [Header("Layer Masks")]
    public LayerMask ObstaclesLayerMask;

    [Header("Physics")]
    /// the speed at which external forces get lerped to zero
    public float ImpactFalloff = 5f;
    /// the force to apply when colliding with rigidbodies
    public float PushPower = 2f;
    /// returns the center coordinate of the collider
    public Vector3 ColliderCenter { get { return this.transform.position + _characterController.center; } }
    /// returns the bottom coordinate of the collider
    public Vector3 ColliderBottom { get { return this.transform.position + _characterController.center + Vector3.down * _characterController.bounds.extents.y; } }
    /// returns the top coordinate of the collider
    public Vector3 ColliderTop { get { return this.transform.position + _characterController.center + Vector3.up * _characterController.bounds.extents.y; } }

    #endregion

    protected Vector3 _positionLastFrame;
    protected Vector3 _impact;
    protected Transform _transform;
    protected Rigidbody _rigidBody;
    protected Collider _collider;
    protected CharacterController _characterController;
    protected Vector3 _groundNormal = Vector3.zero;
    protected Vector3 _lastGroundNormal = Vector3.zero;

    // char movement
    protected CollisionFlags _collisionFlags;
    protected Vector3 _hitPoint = Vector3.zero;
    protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

    // velocity
    protected Vector3 _newVelocity;
    protected Vector3 _lastHorizontalVelocity;
    protected Vector3 _newHorizontalVelocity;
    protected Vector3 _motion;
    protected Vector3 _idealVelocity;
    protected Vector3 _horizontalVelocityDelta;
    protected float _stickyOffset;

    // move position
    protected RaycastHit _movePositionHit;
    protected Vector3 _capsulePoint1;
    protected Vector3 _capsulePoint2;
    protected Vector3 _movePositionDirection;
    protected float _movePositionDistance;

    protected virtual void Awake()
    {
        CurrentDirection = transform.forward;

        Initialization();
    }

    protected virtual void Initialization()
    {
        _characterController = GetComponent<CharacterController>();
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    #region UPDATE METHODS

    protected virtual void Update()
    {
        DetermineDirection();

        if (UpdateMode == UpdateModes.Update)
        {
            ProcessUpdate();
        }
    }

    protected virtual void DetermineDirection()
    {
        if (CurrentMovement.magnitude > 0f)
        {
            CurrentDirection = CurrentMovement.normalized;
        }
    }

    protected virtual void FixedUpdate()
    {
        ApplyImpact();
        if (UpdateMode == UpdateModes.FixedUpdate)
        {
            ProcessUpdate();
        }
    }

    protected virtual void ApplyImpact()
    {
        if(_impact.magnitude > 0.2f)
        {
            _characterController.Move(_impact * Time.deltaTime);
        }
        _impact = Vector3.Lerp(_impact, Vector3.zero, ImpactFalloff * Time.deltaTime);
    }

    protected virtual void ProcessUpdate()
    {
        if (_transform == null) { return; }

        _newVelocity = Velocity;
        _positionLastFrame = _transform.position;

        AddInput();
        ComputeVelocityDelta();
        MoveCharacterController();
        ComputeNewVelocity();
    }

    protected virtual void AddInput()
    {
        _idealVelocity = CurrentMovement;

        _newVelocity = _idealVelocity;
        _newVelocity.y = Mathf.Min(_newVelocity.y, 0);
    }

    /// <summary>
    /// Computes the motion vector to apply to the character controller 
    /// </summary>
    protected virtual void ComputeVelocityDelta()
    {
        //Debug.Log($"{this.GetType()}.ComputeVelocityDelta: _newVelocity = {_newVelocity}.", gameObject);
        _motion = _newVelocity * Time.deltaTime;
        _horizontalVelocityDelta.x = _motion.x;
        _horizontalVelocityDelta.y = 0f;
        _horizontalVelocityDelta.z = _motion.z;
        _stickyOffset = Mathf.Max(_characterController.stepOffset, _horizontalVelocityDelta.magnitude);
        _motion -= _stickyOffset * Vector3.up;
    }

    protected virtual void MoveCharacterController()
    {
        _groundNormal = Vector3.zero;

        //Debug.Log($"{this.GetType()}.MoveCharacterController: _motion = {_motion}.", gameObject);
        _collisionFlags = _characterController.Move(_motion); // controller move

        _lastHitPoint = _hitPoint;
        _lastGroundNormal = _groundNormal;
    }

    /// <summary>
    /// Determines the new Velocity value based on our position and our position last frame
    /// </summary>
    protected virtual void ComputeNewVelocity()
    {
        // Store the horizontal components of the new velocity
        _lastHorizontalVelocity.x = _newVelocity.x;
        _lastHorizontalVelocity.y = 0;
        _lastHorizontalVelocity.z = _newVelocity.z;

        // Calculate the overall velocity of the object using its position change over time
        Velocity = (_transform.position - _positionLastFrame) / Time.deltaTime;

        // Store the horizontal components of the current velocity
        _newHorizontalVelocity.x = Velocity.x;
        _newHorizontalVelocity.y = 0;
        _newHorizontalVelocity.z = Velocity.z;

        // Adjust the velocity based on the previous horizontal velocity
        if (_lastHorizontalVelocity == Vector3.zero)
        {
            // If the previous horizontal velocity is zero, the object is not moving horizontally
            Velocity.x = 0f;
            Velocity.z = 0f;
        }
        else
        {
            // Calculate the new velocity by projecting the current velocity onto the previous horizontal velocity
            float newVelocity = Vector3.Dot(_newHorizontalVelocity, _lastHorizontalVelocity) / _lastHorizontalVelocity.sqrMagnitude;
            // Combine the projected horizontal velocity and the vertical velocity
            Velocity = _lastHorizontalVelocity * Mathf.Clamp01(newVelocity) + Velocity.y * Vector3.up;
        }

        // Adjust the velocity based on the desired vertical velocity (_newVelocity.y)
        if (Velocity.y < _newVelocity.y - 0.001)
        {
            // If the current vertical velocity is lower than the desired velocity with a small threshold,
            // check if it is negative and set it to the desired velocity to prevent it from going further down
            if (Velocity.y < 0)
            {
                Velocity.y = _newVelocity.y;
            }
        }
    }

    protected virtual void LateUpdate()
    {
        ComputeSpeed();
    }

    protected virtual void ComputeSpeed()
    {
        Speed = (this.transform.position - _positionLastFrame) / Time.deltaTime;
        // we round the speed to 2 decimals
        Speed.x = Mathf.Round(Speed.x * 100f) / 100f;
        Speed.y = Mathf.Round(Speed.y * 100f) / 100f;
        Speed.z = Mathf.Round(Speed.z * 100f) / 100f;
        _positionLastFrame = this.transform.position;
    }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Enables the collider
    /// </summary>
    public virtual void CollisionsOn()
    {
        _collider.enabled = true;
    }

    /// <summary>
    /// Disables collider
    /// </summary>
    public virtual void CollisionsOff()
    {
        _collider.enabled = false;
    }

    /// <summary>
    /// Use this to apply an impact to a controller, moving it in the specified direction at the specified force
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public virtual void Impact(Vector3 direction, float force)
    {
        direction = direction.normalized;
        if (direction.y < 0)
        {
            direction.y = -direction.y;
        }
        _impact += direction.normalized * force;
    }

    /// <summary>
    /// Adds the specified force to the controller
    /// </summary>
    /// <param name="movement"></param>
    public virtual void AddForce(Vector3 movement)
    {
        AddedForce += movement;
    }

    /// <summary>
    /// Sets the character's current input direction and magnitude
    /// </summary>
    /// <param name="movement"></param>
    public virtual void SetMovement(Vector3 movement)
    {
        CurrentMovement = movement;

        Vector3 directionVector;
        directionVector = movement;
        if (directionVector != Vector3.zero)
        {
            float directionLength = directionVector.magnitude;
            directionVector /= directionLength;
            directionLength = Mathf.Min(1, directionLength);
            directionLength *= directionLength;
            directionVector *= directionLength;
        }
        InputMoveDirection = transform.rotation * directionVector;
    }

    /// <summary>
    /// Turns this character's rigidbody kinematic or not
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetKinematic(bool state)
    {
        _rigidBody.isKinematic = state;
    }

    /// <summary>
    /// Moves this character to the specified position while trying to avoid obstacles
    /// </summary>
    /// <param name="newPosition"></param>
    public virtual void MovePosition(Vector3 newPosition)
    {

        _movePositionDirection = (newPosition - this.transform.position);
        _movePositionDistance = Vector3.Distance(this.transform.position, newPosition);

        _capsulePoint1 = this.transform.position
                            + _characterController.center
                            - (Vector3.up * _characterController.height / 2f)
                            + Vector3.up * _characterController.skinWidth
                            + Vector3.up * _characterController.radius;
        _capsulePoint2 = this.transform.position
                            + _characterController.center
                            + (Vector3.up * _characterController.height / 2f)
                            - Vector3.up * _characterController.skinWidth
                            - Vector3.up * _characterController.radius;

        if (!Physics.CapsuleCast(_capsulePoint1, _capsulePoint2, _characterController.radius, _movePositionDirection, out _movePositionHit, _movePositionDistance, ObstaclesLayerMask))
        {
            this.transform.position = newPosition;
        }
    }

    /// <summary>
    /// Resets all values for this controller
    /// </summary>
    public virtual void Reset()
    {
        _impact = Vector3.zero;
        Speed = Vector3.zero;
        Velocity = Vector3.zero;
        VelocityLastFrame = Vector3.zero;
        Acceleration = Vector3.zero;
        CurrentMovement = Vector3.zero;
        CurrentDirection = Vector3.zero;
        AddedForce = Vector3.zero;
    }

    #endregion
}
