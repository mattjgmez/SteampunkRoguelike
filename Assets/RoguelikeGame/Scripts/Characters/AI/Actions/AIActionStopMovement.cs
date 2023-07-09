using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class AIActionStopMovement : AIAction
{
    protected CharacterMovement _characterMovement;

    protected override void Initialization()
    {
        _characterMovement = GetComponent<CharacterMovement>();
    }

    public override void PerformAction()
    {
        StopMovement();
    }

    protected virtual void StopMovement()
    {
        _characterMovement.MovementForbidden = true;
    }

    public override void OnExitState()
    {
        base.OnExitState();

        _characterMovement.MovementForbidden = false;
    }
}
