using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUpgradeable : CharacterAbility
{
    public Upgrade[] Upgrades;

    protected bool _upgradesApplied = false;

    protected override void Start()
    {
        base.Start();
        if (UpgradeManager.Instance.Upgrades.Length <= 0) { return; }
        Upgrades = UpgradeManager.Instance.Upgrades;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ApplyUpgrades();

        UpgradeManager.Instance.NewUpgrade += ApplyUpgrades;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnapplyUpgrades();

        UpgradeManager.Instance.NewUpgrade -= ApplyUpgrades;
    }

    public virtual void ApplyUpgrades()
    {
        if (UpgradeManager.Instance.Upgrades.Length <= 0) { return; }

        if (_upgradesApplied)
        {
            UnapplyUpgrades();
        }

        Upgrades = UpgradeManager.Instance.Upgrades;

        for (int i = 0; i < Upgrades.Length; i++)
        {
            Upgrades[i].CalculateBonus();
            Upgrades[i].ApplyUpgrade(_character);
        }

        _upgradesApplied = true;
    }

    private void UnapplyUpgrades()
    {
        if (Upgrades.Length <= 0) { return; }

        for (int i = 0; i < Upgrades.Length; i++)
        {
            Upgrades[i].CalculateBonus();
            Upgrades[i].UnapplyUpgrade(_character);
        }

        _upgradesApplied = false;
    }
}
