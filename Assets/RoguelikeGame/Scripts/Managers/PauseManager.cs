using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    public bool GameOver;

    public bool IsPaused = false;
    public Character Player;

    protected virtual void Update()
    {
        if (InputManager.Instance == null) { return; }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public virtual void TogglePause()
    {
        if (GameOver) { return; }

        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        
        Player.SetEnable(!IsPaused);

        if (UIManager.Instance == null ) { return; }

        UIManager.Instance.SetPauseScreen( IsPaused );
    }

    public virtual void SetPause(bool state)
    {
        if (GameOver) { return; }
        
        IsPaused = state;
        Time.timeScale = IsPaused ? 0f : 1f;

        Player.SetEnable(!IsPaused);
    }
}
