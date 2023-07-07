using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAiming : CharacterAbility
{
    public float RotationSpeed = 10f;

    protected Vector3 _mousePosition;
    protected Camera _mainCamera;
    protected Plane _playerPlane;
    protected Vector3 _direction;
    protected Vector3 _currentAim;

    protected override void Initialization()
    {
        base.Initialization();
        _mainCamera = Camera.main;
        _playerPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public override void ProcessAbility()
    {
        GetMouseAim();
        RotateTowardsMouse();
    }

    public virtual void GetMouseAim()
    {
        _mousePosition = Input.mousePosition;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
        float distance;
        if (_playerPlane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            _direction = target;
        }

        _direction.y = transform.position.y;
        _currentAim = _direction - transform.position;
    }

    public virtual void RotateTowardsMouse()
    {
        // Calculate the rotation needed to point towards the direction
        Quaternion targetRotation = Quaternion.LookRotation(_currentAim);

        // Smoothly interpolate current rotation towards the target rotation
        _model.transform.rotation = Quaternion.Lerp(_model.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

}
