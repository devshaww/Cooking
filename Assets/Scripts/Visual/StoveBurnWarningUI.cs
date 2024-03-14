using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {
        Hide();
        stoveCounter.OnProgressUpdate += StoveCounter_OnProgressUpdate;
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void StoveCounter_OnProgressUpdate(object sender, IProgressible.OnProgressChangeEventArgs e)
    {
        if (stoveCounter.IsFried() && e.progressNormalized >= 0.5f) {            
            Show();
        } else {
            Hide();
        }               
    }
}
