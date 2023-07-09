using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public TMP_Text DamageCounter;
    public TMP_Text DurationCounter;
    public TMP_Text HealthCounter;
    public TMP_Text MovespeedCounter;

    protected virtual void OnEnable()
    {
        if (UpgradeManager.Instance == null) { return; }

        DamageCounter.text = $"x{UpgradeManager.Instance.GetComponent<UpgradeDamage>().AmountActive}";
        DurationCounter.text = $"x{UpgradeManager.Instance.GetComponent<UpgradeAttackDuration>().AmountActive}";
        HealthCounter.text = $"x{UpgradeManager.Instance.GetComponent<UpgradeHealth>().AmountActive}";
        MovespeedCounter.text = $"x{UpgradeManager.Instance.GetComponent<UpgradeMovespeed>().AmountActive}";
    }
}
