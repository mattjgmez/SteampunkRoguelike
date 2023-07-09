using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all UI effects and changes
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public Canvas MainCanvas;
    public GameObject HUD;
    public Slider HealthBar;
    public GameObject PauseScreen;
    public GameObject DeathScreen;
    public GameObject VictoryScreen;
    public TMP_Text PointsText;
    public TMP_Text UpgradeTimer;
    public TMP_Text RemainingEnemies;
    public GameObject UpgradeSelectScreen;
    public UpgradeCard[] UpgradeCards;

    protected virtual void Start()
    {
        SetPauseScreen(false);
        SetDeathScreen(false);
        SetUpgradeSelectScreen(false);
    }

    /// <summary>
    /// Sets the pause screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetPauseScreen(bool state)
    {
        if (PauseScreen != null)
        {
            PauseScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the death screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetDeathScreen(bool state)
    {
        if (DeathScreen != null)
        {
            DeathScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the victory screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetVictoryScreen(bool state)
    {
        if (VictoryScreen != null)
        {
            VictoryScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the upgrade select screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetUpgradeSelectScreen(bool state)
    {
        if (UpgradeSelectScreen != null)
        {
            PauseManager.Instance.SetPause(state);
            if (state == true)
            {
                List<Upgrade> upgrades = new List<Upgrade>(UpgradeManager.Instance.Upgrades);
                foreach (UpgradeCard card in UpgradeCards)
                {
                    int index = UnityEngine.Random.Range(0, upgrades.Count);

                    //Debug.Log($"{this.GetType()}.SetUpgradeSelectScreen: index = {index} cardData = {upgrades[index]}.", gameObject);

                    card.AssignUpgradeData(upgrades[index]);
                    upgrades.Remove(upgrades[index]);
                }
            }
            UpgradeSelectScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    public virtual void TriggerSetUpgradeSelectScreen(bool state)
    {
        StartCoroutine(SetUpgradeSelectScreenCoroutine(state));
    }

    public virtual IEnumerator SetUpgradeSelectScreenCoroutine(bool state)
    {
        yield return new WaitForSeconds(1);
        SetUpgradeSelectScreen(state);
    }

    /// <summary>
    /// Updates the health bar.
    /// </summary>
    /// <param name="currentHealth">Current health.</param>
    /// <param name="minHealth">Minimum health.</param>
    /// <param name="maxHealth">Max health.</param>
    public virtual void UpdateHealthBar(float currentHealth, float minHealth, float maxHealth)
    {
        if (HealthBar == null) { return; }


        HealthBar.minValue = minHealth; 
        HealthBar.maxValue = maxHealth;
        HealthBar.value = currentHealth;
    }

    public virtual void UpdateUpgradeTimer(float time)
    {
        if (time >= 60)
        {
            int minutes = (int)time / 60;
            UpgradeTimer.text = minutes.ToString("D") + "m";
        }
        else
        {
            UpgradeTimer.text = ((int)time).ToString();
        }
    }

    public virtual void UpdateRemainingEnemies(int value)
    {
        RemainingEnemies.text = value.ToString();
    }
}