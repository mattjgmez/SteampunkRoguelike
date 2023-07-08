using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AIDecisionLineOfSightToTarget : AIDecision
{
    /// the layermask to consider as obstacles when trying to determine whether a line of sight is present
    public LayerMask ObstacleLayerMask;
    /// the offset to apply (from the collider's center) when casting a ray from the agent to its target
    public Vector3 LineOfSightOffset = new Vector3(0, 0, 0);

    protected Vector3 _directionToTarget;
    protected Collider _collider;
    protected Vector3 _raycastOrigin;

    public override void Initialization()
    {
        _collider = GetComponent<Collider>();
    }

    public override bool Decide()
    {
        return CheckLineOfSight();
    }

    protected virtual bool CheckLineOfSight()
    {
        if (_brain.Target == null)
        {
            return false;
        }

        _raycastOrigin = _collider.bounds.center + LineOfSightOffset / 2;
        _directionToTarget = _brain.Target.transform.position - _raycastOrigin;

        RaycastHit hit = JP_Debug.Raycast3D(_raycastOrigin, _directionToTarget.normalized, _directionToTarget.magnitude, ObstacleLayerMask, Color.yellow, true);
        if (hit.collider == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
