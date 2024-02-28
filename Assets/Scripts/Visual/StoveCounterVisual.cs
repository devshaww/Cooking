using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject particlesGameObject;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private StoveCounter stoveCounter;

	private void Start()
	{
		stoveCounter.OnStateChange += StoveCounter_OnStateChange;
	}

	private void StoveCounter_OnStateChange(object sender, StoveCounter.OnStateChangeEventArgs e)
	{
		bool showVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
		stoveOnGameObject.SetActive(showVisual);
		particlesGameObject.SetActive(showVisual);
	}
}
