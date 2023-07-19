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
        UpdateRemainingEnemies(GameManager.Instance.EnemyCount);
    }

    /// <summary>
    /// Sets the pause screen on or off.
    /// </summary>
    public virtual void SetPauseScreen(bool state)
    {
        if (PauseScreen != null)
        {
            PauseScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the death screen on or off.
    /// </summary>
    public virtual void SetDeathScreen(bool state)
    {
        if (DeathScreen != null)
        {
            DeathScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the victory screen on or off.
    /// </summary>
    public virtual void SetVictoryScreen(bool state)
    {
        if (VictoryScreen != null)
        {
            VictoryScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the upgrade select screen on or off.
    /// </summary>
    public virtual void SetUpgradeSelectScreen(bool state)
    {
        if (UpgradeSelectScreen != null)
        {
            UpgradeSelectScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            if (state == true)
            {
                List<Upgrade> upgrades = new List<Upgrade>(UpgradeManager.Instance.Upgrades);
                foreach (UpgradeCard card in UpgradeCards)
                {
                    card.Button.interactable = true;
                    card.GetComponent<Image>().raycastTarget = true;
                    card.Animator.SetBool("Selected", false);

                    int index = UnityEngine.Random.Range(0, upgrades.Count);

                    card.AssignUpgradeData(upgrades[index]);
                    upgrades.Remove(upgrades[index]);
                }
            }
            EventSystem.current.sendNavigationEvents = state;
        }
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


    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }

    public void RestartRun()
    {
        GameManager.Instance.RestartRun();
    }

    public void CloseGame()
    {
        GameManager.Instance.CloseGame();
    }
}
