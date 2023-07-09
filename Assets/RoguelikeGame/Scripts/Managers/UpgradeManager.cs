using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages active Upgrades and the Upgrade selection timer.
/// Carries Upgrades between scenes.
/// </summary>
public class UpgradeManager : PersistentSingleton<UpgradeManager>
{
    public Upgrade[] Upgrades;
    public float TimeInSecondsBetweenUpgrades = 300;

    public delegate void UpgradeHandler();
    public event UpgradeHandler NewUpgrade;

    protected int[] _possibleUpgradesIndex;
    protected float _remainingTime = 0;
    protected bool _timerIsRunning;

    protected virtual void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        Upgrades = GetComponents<Upgrade>();
        _possibleUpgradesIndex = new int[Upgrades.Length];
        for (int i = 0; i < _possibleUpgradesIndex.Length; i++)
        {
            _possibleUpgradesIndex[i] = i;
        }
        _timerIsRunning = true;
        _remainingTime = TimeInSecondsBetweenUpgrades;
    }

    protected virtual void Update()
    {
        HandleTimer();
    }

    protected virtual void HandleTimer()
    {
        if (!_timerIsRunning) { return; }

        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateUpgradeTimer(_remainingTime);
            }
        }
        else
        {
            _remainingTime = 0;
            _timerIsRunning = false;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetUpgradeSelectScreen(true);
            }
        }
    }

    public virtual void RestartTimer()
    {
        _remainingTime = TimeInSecondsBetweenUpgrades;
        _timerIsRunning = true;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUpgradeTimer(_remainingTime);
        }
    }

    public virtual void AddUpgrade(string label)
    {
        foreach (Upgrade upgrade in Upgrades)
        {
            if (upgrade.Label == label)
            {
                upgrade.AmountActive++;
                NewUpgrade?.Invoke();
                break;
            }
        }
    }
}
