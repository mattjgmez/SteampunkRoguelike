using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : PersistentSingleton<UpgradeManager>
{
    public Upgrade[] PossibleUpgrades;
    public Upgrade[] ActiveUpgrades;
    public float TimeInSecondsBetweenUpgrades = 300;

    public delegate void UpgradeHandler();
    public event UpgradeHandler UpgradeApplied;

    protected int[] _possibleUpgradesIndex;
    protected int _currentTimer = 0;


}
