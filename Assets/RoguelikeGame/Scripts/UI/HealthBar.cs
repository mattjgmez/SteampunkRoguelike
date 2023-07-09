using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Health PlayerHealth;

    public virtual void Awake()
    {
        if (PlayerHealth == null)
        {
            PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }
        if (PlayerHealth != null)
        {
            UpdateHealthBarUI();
        }
    }

    protected virtual void OnEnable()
    {
        Debug.Log($"{this.GetType()}.OnEnable: Attempting to subscribe to player. PlayerHealth found = {PlayerHealth != null}", gameObject);
        if (PlayerHealth != null)
        {
            PlayerHealth.OnHealthChange += UpdateHealthBarUI;
        }
    }

    protected virtual void OnDisable()
    {
        if (PlayerHealth != null)
        {
            PlayerHealth.OnHealthChange -= UpdateHealthBarUI;
        }
    }

    public virtual void UpdateHealthBarUI()
    {
        Debug.Log($"{this.GetType()}.UpdateHealthBarUI: Attempting to update health bar.", gameObject);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthBar(PlayerHealth.CurrentHealth, 0, PlayerHealth.MaxHealth);
        }
    }
}
