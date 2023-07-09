using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public virtual void PlayFootstep()
    {
        AudioManager.Instance.PlayRandomClip(AudioManager.Instance.FootstepClips);
    }

    public virtual void PlayGunshot()
    {
        AudioManager.Instance.PlayRandomClip(AudioManager.Instance.GunshotClips);
    }

    public virtual void PlayRobotSpin()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.RobotSpinClip);
    }

    public virtual void PlayRobotDeath()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.RobotDeathClip);
    }

    public virtual void PlayPlayerDeath()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.PlayerDeathClip);
    }
}
