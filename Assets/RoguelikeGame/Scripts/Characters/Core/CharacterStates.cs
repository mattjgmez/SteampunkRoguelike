using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    /// The possible character conditions
    public enum CharacterConditions
    {
        Normal,
        ControlledMovement,
        Paused,
        Dead
    }

    /// The possible Movement States the character can be in. These usually correspond to their own class, 
    /// but it's not mandatory
    public enum MovementStates
    {
        Null,
        Idle,
        Walking,
        Dashing,
        Attacking,
    }
}
