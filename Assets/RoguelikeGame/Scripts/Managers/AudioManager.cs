using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    public AudioSource AudioSource { get; private set; }

    public AudioClip[] FootstepClips;
    public AudioClip[] GunshotClips;
    public AudioClip[] PlayerTakeDamageClips;
    public AudioClip[] BulletHitClips;
    public AudioClip RobotSpinClip;
    public AudioClip PlayerDeathClip;
    public AudioClip RobotDeathClip;

    protected override void Awake()
    {
        base.Awake();
        Initialization();
    }

    protected virtual void Initialization()
    {
        this.AudioSource = GetComponent<AudioSource>();
    }

    public virtual void PlayClip(AudioClip clip)
    {
        this.AudioSource.PlayOneShot(clip);
    }

    public virtual void PlayRandomClip(AudioClip[] clipArray)
    {
        this.AudioSource.PlayOneShot(clipArray[Random.Range(0, clipArray.Length)]);
    }
}
