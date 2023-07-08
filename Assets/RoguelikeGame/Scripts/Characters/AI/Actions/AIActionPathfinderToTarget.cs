using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class AIActionPathfinderToTarget : AIAction
{
    protected CharacterMovement _characterMovement;
    protected CharacterPathfinder _characterPathfinder;

    protected override void Initialization()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _characterPathfinder = GetComponent<CharacterPathfinder>();
    }

    public override void PerformAction()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (_brain.Target == null)
        {
            _characterPathfinder.SetNewDestination(null);
            return;
        }
        else
        {
            _characterPathfinder.SetNewDestination(_brain.Target.transform);
        }
    }

    public override void OnExitState()
    {
        base.OnExitState();

        _characterMovement.SetMovement(Vector2.zero);
    }
}
