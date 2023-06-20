using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    public float MovementSpeed = 6.0f;

    protected CharacterController _controller;
    protected Vector3 _moveDirection = Vector3.zero;

    protected void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    protected void Update()
    {
        // Get WASD input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Create a new Vector3 to apply movement in the X and Z directions
        _moveDirection = new Vector3(moveX, 0, moveZ);
        _moveDirection = transform.TransformDirection(_moveDirection);
        _moveDirection *= MovementSpeed;

        // Move the _controller
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}
