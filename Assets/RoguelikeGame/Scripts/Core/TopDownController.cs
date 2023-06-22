using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
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

    /// <summary>
    /// Use this to apply an impact to a controller, moving it in the specified direction at the specified force
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public virtual void Impact(Vector3 direction, float force)
    {
        direction = direction.normalized;
        if(direction.y < 0)
        {
            direction.y = -direction.y;
        }
        _impact += direction.normalized * force;
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
    /// Resets all values for this controller, though currently only resets Impact
    /// </summary>
    public virtual void Reset()
    {
        _impact = Vector3.zero;
    }
}
