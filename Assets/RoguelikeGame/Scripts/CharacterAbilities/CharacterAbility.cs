using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
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
    //protected Health _health;
    protected Animator _animator = null;
    protected bool _abilityInitialized = false;
    protected float _verticalInput;
    protected float _horizontalInput;

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
        _model = _character.CharacterModel;
        //_health = GetComponent<Health>();

        _abilityInitialized = true;
    }

    /// <summary>
    /// Functions as Update in our ability.
    /// </summary>
    public virtual void ProcessAbility()
    {

    }
}
