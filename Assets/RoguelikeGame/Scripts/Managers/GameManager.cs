using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    public GameObject EnemyContainer;
    public int EnemyCount;
    public int CurrentSceneIndex = 0;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        EnemyContainer = GameObject.FindGameObjectWithTag("EnemyContainer");

        if (EnemyContainer == null) { return; }
        
        foreach (Transform enemy in EnemyContainer.transform)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                EnemyCount++;
            }
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateRemainingEnemies(EnemyCount);
        }
    }

    public virtual void UpdateEnemyCount(int value)
    {
        EnemyCount += value;

        if (UIManager.Instance == null) { return; }
        if (EnemyCount <= 0)
        {
            UIManager.Instance.SetVictoryScreen(true);
        }

        UIManager.Instance.UpdateRemainingEnemies(EnemyCount);
    }

    public virtual void NextScene()
    {
        CurrentSceneIndex++;

        if (CurrentSceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            CurrentSceneIndex = 0;
            Destroy(UpgradeManager.Instance.gameObject);
        }

        SceneManager.LoadScene(CurrentSceneIndex);
    }

    public virtual void LoadMainMenu()
    {
        if (UpgradeManager.Instance != null)
        {
            Destroy(UpgradeManager.Instance.gameObject);
        }
        SceneManager.LoadScene("MainMenu");
    }

    public virtual void RestartRun()
    {
        if (UpgradeManager.Instance != null)
        {
            Destroy(UpgradeManager.Instance.gameObject);
        }
        SceneManager.LoadScene("Floor1");
    }

    public virtual void CloseGame()
    {
        Application.Quit();
    }

    public virtual void TriggerGameOver()
    {
        if (UIManager.Instance == null) { return; }

        UIManager.Instance.SetDeathScreen(true);
    }
}