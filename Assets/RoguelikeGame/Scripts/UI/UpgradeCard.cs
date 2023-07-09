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

    public virtual void AssignUpgradeData(Upgrade upgradeToAssign)
    {
        //Debug.Log($"{this.GetType()}.AssignUpgradeData: {Button}.", gameObject);

        UpgradeData = upgradeToAssign;
        Button.onClick.AddListener(() => UpgradeManager.Instance.AddUpgrade(UpgradeData.Label));

        Icon.sprite = UpgradeData.Sprite;
        NameText.text = UpgradeData.Label;
        DescriptionText.text = UpgradeData.Description;
    }
}
