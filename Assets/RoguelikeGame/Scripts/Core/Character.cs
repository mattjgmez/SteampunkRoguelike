using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    /// the possible character types : player _controller or AI (controlled by the computer)
    public enum CharacterTypes { Player, AI }

    /// Is the character player-controlled or controlled by an AI ?
    public CharacterTypes CharacterType = CharacterTypes.AI;
    // Used if the character is player controller. The PlayerID must match the Input Manager's PlayerID.
    public string PlayerID = "";

    public GameObject CharacterModel;

    protected CharacterAbility[] _characterAbilities;

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _characterAbilities = GetComponents<CharacterAbility>();
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
