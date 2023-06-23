using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    public Vector3 Speed;
    public Vector3 AddedForce;

    [Header("Physics")]
    // the speed at which external forces get lerped to zero
    public float ImpactFalloff = 5f;
    /// returns the center coordinate of the collider
    public Vector3 ColliderCenter { get { return this.transform.position + _controller.center; } }
    /// returns the bottom coordinate of the collider
    public Vector3 ColliderBottom { get { return this.transform.position + _controller.center + Vector3.down * _controller.bounds.extents.y; } }
    /// returns the top coordinate of the collider
    public Vector3 ColliderTop { get { return this.transform.position + _controller.center + Vector3.up * _controller.bounds.extents.y; } }

    public float MovementSpeed = 6.0f;

    protected Vector3 _positionLastFrame;
    protected Transform _transform;
    protected Rigidbody _rigidBody;
    protected Collider _collider;
    protected CharacterController _controller;
    protected Vector3 _impact;
    protected Vector3 _moveDirection = Vector3.zero;

    protected void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _controller = GetComponent<CharacterController>();
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        HandleMovement();
    }

    protected virtual void FixedUpdate()
    {
        ApplyImpact();
    }

    protected virtual void LateUpdate()
    {
        ComputeSpeed();
    }

    protected virtual void HandleMovement()
    {
        // Get WASD input
        float moveX = InputManager.Instance.PrimaryMovement.x;
        float moveZ = InputManager.Instance.PrimaryMovement.y;

        // Create a new Vector3 to apply movement in the X and Z directions
        _moveDirection = new Vector3(moveX, 0, moveZ);
        _moveDirection = transform.TransformDirection(_moveDirection);
        _moveDirection *= MovementSpeed;

        // Move the _controller
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    protected virtual void ApplyImpact()
    {
        if(_impact.magnitude > 0.2f)
        {
            _controller.Move(_impact * Time.deltaTime);
        }
        _impact = Vector3.Lerp(_impact, Vector3.zero, ImpactFalloff * Time.deltaTime);
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

    #region PUBLIC METHODS

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
    /// Resets all values for this controller
    /// </summary>
    public virtual void Reset()
    {
        _impact = Vector3.zero;
        Speed = Vector3.zero;
        AddedForce = Vector3.zero;
    }

    #endregion
}
