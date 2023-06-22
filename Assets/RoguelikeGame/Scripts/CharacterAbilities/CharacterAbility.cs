using Phoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAbility : MonoBehaviour
{
    #region VARIABLES

    /// the sound fx to play when the ability starts
    public AudioClip AbilityStartSfx;
    /// the sound fx to play while the ability is running
    public AudioClip AbilityInProgressSfx;
    /// the sound fx to play when the ability stops
    public AudioClip AbilityStopSfx;

    /// whether or not this ability has been initialized
    public bool AbilityInitialized { get { return _abilityInitialized; } }

    protected Character _character;
    protected TopDownController _controller;
    protected GameObject _model;
    protected Health _health;
    protected InputManager _inputManager;
    protected Animator _animator = null;
    protected CharacterStates _state;
    protected StateMachine<CharacterStates.MovementStates> _movement;
    protected StateMachine<CharacterStates.CharacterConditions> _condition;
    protected bool _abilityInitialized = false;
    protected float _verticalInput;
    protected float _horizontalInput;

    #endregion

    /// <summary>
    /// On awake we proceed to pre initializing our ability
    /// </summary>
    protected virtual void Awake()
    {
        PreInitialization();
    }

    /// <summary>
    /// On Start(), we call the ability's intialization
    /// </summary>
    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void PreInitialization()
    {
        _character = GetComponent<Character>();
    }

    protected virtual void Initialization()
    {
        _controller = GetComponent<TopDownController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();

        _model = _character.CharacterModel;
        _inputManager = _character.LinkedInputManager;
        _state = _character.CharacterState;
        _movement = _character.MovementState;
        _condition = _character.ConditionState;

        _abilityInitialized = true;
    }

    /// <summary>
    /// Internal method to check if an input manager is present or not
    /// </summary>
    protected virtual void InternalHandleInput()
    {
        if (_inputManager == null) { return; }
        _horizontalInput = _inputManager.PrimaryMovement.x;
        _verticalInput = _inputManager.PrimaryMovement.y;
        HandleInput();
    }

    /// <summary>
    /// Changes the reference to the input manager with the one set in parameters
    /// </summary>
    public virtual void SetInputManager(InputManager newInputManager)
    {
        _inputManager = newInputManager;
    }

    #region PLACEHOLDER METHODS

    /// <summary>
    /// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls methods if conditions are met
    /// </summary>
    protected virtual void HandleInput() { }

    /// <summary>
    /// Resets all input for this ability. Can be overridden for ability specific directives
    /// </summary>
    public virtual void ResetInput()
    {
        _horizontalInput = 0f;
        _verticalInput = 0f;
    }

    /// <summary>
    /// Functions as EarlyUpdate in our ability (if it existed).
    /// </summary>
    public virtual void EarlyProcessAbility() { }

    /// <summary>
    /// Functions as Update in our ability.
    /// </summary>
    public virtual void ProcessAbility() { }

    /// <summary>
    /// Functions as LateUpdate in our ability.
    /// </summary>
    public virtual void LateProcessAbility() 
    {
        InternalHandleInput();
    }

    /// <summary>
    /// Override this to reset this ability's parameters. It'll be automatically called when the character gets killed, in anticipation for its respawn.
    /// </summary>
    public virtual void ResetAbility() { }

    /// <summary>
    /// Override this to describe what should happen to this ability when the character takes a hit
    /// </summary>
    protected virtual void OnHit() { }

    /// <summary>
    /// Override this to describe what should happen to this ability when the character respawns
    /// </summary>
    protected virtual void OnDeath() { }

    /// <summary>
    /// Override this to describe what should happen to this ability when the character respawns
    /// </summary>
    protected virtual void OnRespawn() { }

    #endregion

    /// <summary>
    /// On enable, we bind our respawn delegate
    /// </summary>
    protected virtual void OnEnable()
    {
        if (_health == null)
        {
            _health = GetComponent<Health>();
        }

        if (_health != null)
        {
            _health.OnRevive += OnRespawn;
            _health.OnDeath += OnDeath;
            _health.OnHit += OnHit;
        }
    }

    /// <summary>
    /// On disable, we unbind our respawn delegate
    /// </summary>
    protected virtual void OnDisable()
    {
        if (_health != null)
        {
            _health.OnRevive -= OnRespawn;
            _health.OnDeath -= OnDeath;
            _health.OnHit -= OnHit;
        }
    }
}
