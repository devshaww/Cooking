using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] visualGameObjects;
    [SerializeField] private BaseCounter counter;

	private void Start()
	{
		if (Player.LocalInstance != null) {
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		} else { // 到这里如果Player Network还未生成 即OnAnyPlayerSpawned还未被Invoke 则确保事件会被绑定上
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
		}
	}

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null) {
			Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged; // ???
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		}
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
