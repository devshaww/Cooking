using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;

    private bool playWarningSound;
    private float warningSoundTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStateChange += StoveCounter_OnStateChange;
        stoveCounter.OnProgressUpdate += StoveCounter_OnProgressUpdate;
    }

    private void StoveCounter_OnProgressUpdate(object sender, IProgressible.OnProgressChangeEventArgs e)
    {
        if (stoveCounter.IsFried() && e.progressNormalized >= 0.5f) {
			playWarningSound = true;
        }
    }

    private void StoveCounter_OnStateChange(object sender, StoveCounter.OnStateChangeEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSound) {
            audioSource.Play();
        } else {
            audioSource.Pause();
        }
    }

    private void Update()
    {
        if (playWarningSound) {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f) {
                warningSoundTimer = .2f;
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
    }
}
