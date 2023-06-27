using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // The model to disable (if set to).
    public GameObject Model;

    // The current health of the character.
    public int CurrentHealth;
    // If true, this object will not take damage.
    public bool Invulnerable = false;

    [Header("Health")]
    public int MaxHealth;
    public int InitialHealth;

    [Header("Damage")]
    public bool ImmuneToKnockback = false;

    [Header("Death")]
    public bool DestroyOnDeath = true;
    public float DelayBeforeDestruction = 0f;
    public int PointsWhenDestroyed;
    public bool RespawnAtInitialLocation = false;
    public bool DisableControllerOnDeath = true;
    public bool DisableModelOnDeath = true;
    public bool DisableCollisionsOnDeath = true;

    // hit delegate
    public delegate void OnHitDelegate();
    public OnHitDelegate OnHit;

    // respawn delegate
    public delegate void OnReviveDelegate();
    public OnReviveDelegate OnRevive;

    // death delegate
    public delegate void OnDeathDelegate();
    public OnDeathDelegate OnDeath;

    protected Vector3 _initialPosition;
    protected Renderer _renderer;
    protected Character _character;
    protected TopDownController _controller;
    //protected HealthBar _healthBar; THIS NEEDS TO BE IMPLEMENTED EVENTUALLY BUT NOT RIGHT NOW
    protected Collider _collider;
    protected CharacterController _characterController;
    protected bool _initialized = false;
    
    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _character = GetComponent<Character>();
        if (Model != null)
        {
            Model.SetActive(true);
        }

        if (gameObject.GetComponent<Renderer>() != null)
        {
            _renderer = GetComponent<Renderer>();
        }
        if (_character != null)
        {
            if (_character.CharacterModel != null)
            {
                if (_character.CharacterModel.GetComponentInChildren<Renderer>() != null)
                {
                    _renderer = _character.CharacterModel.GetComponentInChildren<Renderer>();
                }
            }
        }

        _controller = GetComponent<TopDownController>();
        _characterController = GetComponent<CharacterController>();
        _collider = GetComponent<Collider>();

        _initialPosition = transform.position;
        _initialized = true;
        CurrentHealth = InitialHealth;
        DamageEnabled();
        UpdateHealthBar(false);
    }

    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public virtual void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration)
    {
        // If the object is invulnerable, or we're already below zero, we do nothing and exit.
        if (Invulnerable || ((CurrentHealth <= 0) && (InitialHealth != 0)))
        {
            return;
        }

        float previousHealth = CurrentHealth;
        CurrentHealth -= damage;

        OnHit?.Invoke();

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabledCoroutine(invincibilityDuration));
        }

        // we trigger a damage taken event
        CharacterEvents.DamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        // we update the health bar
        UpdateHealthBar(true);

        // if health has reached zero
        if (CurrentHealth <= 0)
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;

            Kill();
        }
    }

    /// <summary>
    /// Kills the character, instantiates death effects, handles points, etc
    /// </summary>
    public virtual void Kill()
    {
        if (_character != null)
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
            _character.Reset();

            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                // EVENTUALLY ADD GAMEMANAGER EVENT TRIGGER FOR PLAYER DEATH
            }
        }
        CurrentHealth = 0;

        DamageDisabled();

        if (PointsWhenDestroyed != 0)
        {
            // ADD GAMEMANAGER EVENT FOR GAINING POINTS?
        }

        if (DisableCollisionsOnDeath)
        {
            if (_collider != null)
            {
                _collider.enabled = false;
            }
            if (_controller != null)
            {
                _controller.CollisionsOff();
            }
        }

        OnDeath?.Invoke();

        if (DisableControllerOnDeath && (_controller != null))
        {
            _controller.enabled = false;
            //_characterController.SetKinematic(true);
        }

        if (DisableControllerOnDeath && (_characterController != null))
        {
            _characterController.enabled = false;
        }

        if (DisableModelOnDeath && (Model != null))
        {
            Model.SetActive(false);
        }

        if (DelayBeforeDestruction > 0f)
        {
            Invoke("DestroyObject", DelayBeforeDestruction);
        }
        else
        {
            // finally we destroy the object
            DestroyObject();
        }
    }

    /// <summary>
    /// Destroys the object, or tries to, depending on the character's settings
    /// </summary>
    protected virtual void DestroyObject()
    {
        if (!DestroyOnDeath)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Revive this object.
    /// </summary>
    public virtual void Revive()
    {
        if (!_initialized)
        {
            return;
        }

        if (_collider != null)
        {
            _collider.enabled = true;
        }
        if (_characterController != null)
        {
            _characterController.enabled = true;
        }
        if (_controller != null)
        {
            _controller.enabled = true;
            _controller.CollisionsOn();
            _controller.Reset();
        }
        if (_character != null)
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        }

        if (RespawnAtInitialLocation)
        {
            transform.position = _initialPosition;
        }
        //if (_healthBar != null)
        //{
        //    _healthBar.Initialization();
        //}

        Initialization();
        UpdateHealthBar(false);

        OnRevive?.Invoke();
    }

    /// <summary>
    /// Called when the character gets health (from a stimpack for example)
    /// </summary>
    /// <param name="health">The health the character gets.</param>
    /// <param name="instigator">The thing that gives the character health.</param>
    public virtual void GetHealth(int health, GameObject instigator)
    {
        // this function adds health to the character's Health and prevents it to go above MaxHealth.
        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);
        UpdateHealthBar(true);
    }

    /// <summary>
    /// Resets the character's health to its max value
    /// </summary>
    public virtual void ResetHealthToMaxHealth()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar(false);
    }

    /// <summary>
    /// Updates the character's health bar progress.
    /// </summary>
    protected virtual void UpdateHealthBar(bool show)
    {
        //if (_healthBar != null)
        //{
        //    _healthBar.UpdateBar(CurrentHealth, 0f, MaxHealth, show);
        //}

        /// UNCOMMENT ONCE GUIMANAGER IS IMPLEMENTED
        //if (_character != null)
        //{
        //    if (_character.CharacterType == Character.CharacterTypes.Player)
        //    {
        //        // We update the health bar
        //        if (GUIManager.Instance != null)
        //        {
        //            GUIManager.Instance.UpdateHealthBar(CurrentHealth, 0f, MaxHealth, _character.PlayerID);
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Prevents the character from taking any damage.
    /// </summary>
    public virtual void DamageDisabled()
    {
        Invulnerable = true;
    }

    /// <summary>
    /// Allows the character to take damage.
    /// </summary>
    public virtual void DamageEnabled()
    {
        Invulnerable = false;
    }

    /// <summary>
    /// Makes the character able to take damage again after the specified delay.
    /// </summary>
    /// <returns>The layer collision.</returns>
    public virtual IEnumerator DamageEnabledCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Invulnerable = false;
    }

    protected virtual void OnEnable()
    {
        CurrentHealth = InitialHealth;
        if (Model != null)
        {
            Model.SetActive(true);
        }
        DamageEnabled();
        UpdateHealthBar(false);
    }

    /// <summary>
    /// On Disable, we prevent any delayed destruction from running
    /// </summary>
    protected virtual void OnDisable()
    {
        CancelInvoke();
    }
}
