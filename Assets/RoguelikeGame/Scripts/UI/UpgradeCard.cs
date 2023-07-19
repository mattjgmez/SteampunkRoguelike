using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public Upgrade UpgradeData;
    public Button Button;
    public Animator Animator;

    public Image Icon;
    public TMP_Text NameText;
    public TMP_Text DescriptionText;

    protected virtual void Start()
    {
        Button = GetComponent<Button>();
        Animator = GetComponent<Animator>();
    }

    protected virtual void Initialize()
    {

    }

    public virtual void AssignUpgradeData(Upgrade upgradeToAssign)
    {
        //Debug.Log($"{this.GetType()}.AssignUpgradeData: {Button}.", gameObject);

        UpgradeData = upgradeToAssign;

        Icon.sprite = UpgradeData.Sprite;
        NameText.text = UpgradeData.Label;
        DescriptionText.text = UpgradeData.Description;
    }

    /// A bit of a band-aid method used to call the UpgradeManager's RestartTimer method using an Animation event.
    public virtual void TriggerSelected()
    {
        UpgradeManager.Instance.RestartTimer();
        UpgradeManager.Instance.AddUpgrade(UpgradeData.Label);

        UIManager.Instance.SetUpgradeSelectScreen(false);

        PauseManager.Instance.SetPause(false);
    }
}
