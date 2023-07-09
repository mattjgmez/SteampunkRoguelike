using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
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
