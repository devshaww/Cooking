using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] visualGameObjects;
    [SerializeField] private BaseCounter counter;

	private void Start()
	{
		//Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
	}

	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
	{
		if (e.selectedCounter == counter)
		{
			foreach (GameObject go in visualGameObjects) {
				go.SetActive(true);
			}
		}
		else
		{
			foreach (GameObject go in visualGameObjects)
			{
				go.SetActive(false);
			}
		}
	}
}
