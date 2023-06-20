using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    /// the possible character types : player _controller or AI (controlled by the computer)
    public enum CharacterTypes { Player, AI }

    /// Is the character player-controlled or controlled by an AI ?
    public CharacterTypes CharacterType = CharacterTypes.AI;

    public GameObject CharacterModel;

    protected CharacterAbility[] _characterAbilities;

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {

    }

    protected void Update()
    {
        // Process abilities
        ProcessAbilities();
    }

    /// <summary>
    /// Calls all registered abilities' Process methods
    /// </summary>
    protected virtual void ProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.ProcessAbility();
            }
        }
    }
}
