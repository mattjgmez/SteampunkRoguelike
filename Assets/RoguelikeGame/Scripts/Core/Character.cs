using Phoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region VARIABLES

    /// the possible initial facing direction for your character
    public enum FacingDirections { West, East, North, South }
    /// the possible directions you can force your character to look at after its spawn
    public enum SpawnFacingDirections { Default, Left, Right, Up, Down }
    /// the possible character types : player _controller or AI (controlled by the computer)
    public enum CharacterTypes { Player, AI }
    /// Is the character player-controlled or controlled by an AI ?
    public CharacterTypes CharacterType = CharacterTypes.AI;
    /// Used if the character is player controller. The PlayerID must match the Input Manager's PlayerID.
    public string PlayerID = "";

    public CharacterStates CharacterState { get; protected set; }

    public SpawnFacingDirections DirectionOnSpawn { get; protected set; }

    [Header("Model")]
    public GameObject CharacterModel;

    /// State Machines
    public StateMachine<CharacterStates.MovementStates> MovementState;
    public StateMachine<CharacterStates.CharacterConditions> ConditionState;

    [Header("Events")]
    public bool SendStateChangeEvents = true;
    public bool SendStateUpdateEvents = true;

    public InputManager LinkedInputManager { get; protected set; }

    protected CharacterAbility[] _characterAbilities;
    protected Health _health;
    protected bool _spawnDirectionForced = false;
    protected TopDownController _controller;

    #endregion

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        // Initialize state machines
        MovementState = new StateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
        ConditionState = new StateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);

        // Get the current input manager
        SetInputManager();

        // Initialize components
        _characterAbilities = GetComponents<CharacterAbility>();
        _controller = GetComponent<TopDownController>();
        _health = GetComponent<Health>();
    }

    public virtual void SetInputManager()
    {
        if (CharacterType == CharacterTypes.AI)
        {
            LinkedInputManager = null;
            UpdateAbilitiesInputManagers();
            return;
        }

        if (!string.IsNullOrEmpty(PlayerID))
        {
            LinkedInputManager = null;
            InputManager[] foundInputManagers = FindObjectsOfType(typeof(InputManager)) as InputManager[];
            foreach (InputManager foundInputManager in foundInputManagers)
            {
                if (foundInputManager.PlayerID == PlayerID)
                {
                    LinkedInputManager = foundInputManager;
                }
            }
        }

        UpdateAbilitiesInputManagers();
    }

    public virtual void SetInputManager(InputManager inputManager)
    {
        LinkedInputManager = inputManager;
        UpdateAbilitiesInputManagers();
    }

    protected virtual void UpdateAbilitiesInputManagers()
    {
        if (_characterAbilities == null)
        {
            return;
        }
        for (int i = 0; i < _characterAbilities.Length; i++)
        {
            _characterAbilities[i].SetInputManager(LinkedInputManager);
        }
    }

    /// <summary>
    /// Sets the player ID
    /// </summary>
    /// <param name="newPlayerID">New player ID.</param>
    public virtual void SetPlayerID(string newPlayerID)
    {
        PlayerID = newPlayerID;
        SetInputManager();
    }

    /// <summary>
    /// This is called every frame.
    /// </summary>
    protected virtual void Update()
    {
        EveryFrame();
    }

    /// <summary>
    /// We do this every frame. This is separate from Update for more flexibility.
    /// </summary>
    protected virtual void EveryFrame()
    {
        // we process our abilities
        EarlyProcessAbilities();
        ProcessAbilities();
        LateProcessAbilities();
    }

    /// <summary>
    /// Calls all registered abilities' Early Process methods
    /// </summary>
    protected virtual void EarlyProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.EarlyProcessAbility();
            }
        }
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

    /// <summary>
    /// Calls all registered abilities' Late Process methods
    /// </summary>
    protected virtual void LateProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.LateProcessAbility();
            }
        }
    }

    /// <summary>
    /// Called to disable the player (at the end of a level for example. 
    /// It won't move and respond to input after this.
    /// </summary>
    public virtual void Disable()
    {
        this.enabled = false;
        _controller.enabled = false;
    }

    /// <summary>
    /// Called when the Character dies. 
    /// Calls every abilities' Reset() method, so you can restore settings to their original value if needed
    /// </summary>
    public virtual void Reset()
    {
        _spawnDirectionForced = false;
        if (_characterAbilities == null)
        {
            return;
        }
        if (_characterAbilities.Length == 0)
        {
            return;
        }
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled)
            {
                ability.ResetAbility();
            }
        }
    }
}
