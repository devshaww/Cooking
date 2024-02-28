using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private PlatesCounter platesCounter;

	private List<GameObject> plateVisualList;
	private float plateOffsetY = .1f;

	private void Awake()
	{
		plateVisualList = new List<GameObject>();
	}

	private void Start()
	{
		platesCounter.OnPlateSpawn += PlatesCounter_OnPlateSpawn;
		platesCounter.OnPlateTaken += PlatesCounter_OnPlateTaken;
	}

	private void PlatesCounter_OnPlateTaken(object sender, System.EventArgs e)
	{
		GameObject plateVisualToRemove = plateVisualList[plateVisualList.Count - 1];
		plateVisualList.Remove(plateVisualToRemove);
		Destroy(plateVisualToRemove);
	}

	private void PlatesCounter_OnPlateSpawn(object sender, System.EventArgs e)
	{
		Transform plateVisualTransform = Instantiate(plateVisualPrefab, spawnPoint);
		plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualList.Count, 0);
		plateVisualList.Add(plateVisualTransform.gameObject);
	}
}
