using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioTrigger { Rotate, Jump, Attach, Detach, Collide }

public class AudioObserver : MonoBehaviour
{
    [SerializeField]
    private AudioClip rotate1Clip;
    [SerializeField]
    private AudioClip rotate2Clip;
    [SerializeField]
    private AudioClip jumpClip;
    [SerializeField]
    private AudioClip attachClip;
    [SerializeField]
    private AudioClip collideClip;

    [SerializeField]
    AudioSource loopingAudioSource;
    [SerializeField]
    AudioSource detachingAudioSource;

    public void NotifyAudioTrigger(AudioTrigger notifiedTrigger)
    {
        switch (notifiedTrigger)
        {
            case AudioTrigger.Rotate:
                this.PlayRotateSounds();
                break;
            case AudioTrigger.Jump:
                this.PlayOneOff(this.jumpClip);
                break;
            case AudioTrigger.Attach:
                this.PlayAttachSounds();
                break;
            case AudioTrigger.Detach:
                this.PlayDetachSounds();
                break;
            case AudioTrigger.Collide:
                this.PlayOneOff(this.collideClip);
                break;
        }
    }

    private void PlayRotateSounds()
    {
        this.StartCoroutine(PlayTimedRotateSounds());
    }

    private IEnumerator PlayTimedRotateSounds()
    {
        this.PlayOneOff(this.rotate1Clip);
        yield return new WaitForSeconds(0.2f);
        this.PlayOneOff(this.rotate2Clip);
    }

    private void PlayAttachSounds()
    {
        this.PlayOneOff(this.attachClip);

        this.ModulateLoopingPitch();

        if (this.loopingAudioSource.isPlaying == false)
        {
            this.loopingAudioSource.Play();
        }
    }

    private void PlayDetachSounds()
    {
        if (this.loopingAudioSource.isPlaying == true)
        {
            this.loopingAudioSource.Stop();
        }

        this.detachingAudioSource.Play();       
    }

    private void PlayOneOff(AudioClip oneOffClip)
    {
        AudioSource.PlayClipAtPoint(oneOffClip, this.gameObject.transform.position);
    }

    private void ModulateLoopingPitch()
    {
        this.loopingAudioSource.pitch = Random.Range(1.0f, 1.2f);
        this.detachingAudioSource.pitch = this.loopingAudioSource.pitch;
    }    
}
