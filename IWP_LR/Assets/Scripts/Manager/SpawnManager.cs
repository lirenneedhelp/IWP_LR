using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager Instance;
	private List<Spawnpoint> availableSpawnpoints = new List<Spawnpoint>();


	Spawnpoint[] spawnpoints;

	void Awake()
	{
		Instance = this;
		spawnpoints = GetComponentsInChildren<Spawnpoint>();
		availableSpawnpoints.AddRange(spawnpoints);

	}

	public Transform GetSpawnpoint()
	{
		int randomIndex = Random.Range(0, availableSpawnpoints.Count);
		Transform selectedSpawnpoint = availableSpawnpoints[randomIndex].transform;

		// Remove the selected spawn point from the availableSpawnpoints list
		availableSpawnpoints.RemoveAt(randomIndex);

		Debug.Log(randomIndex);

		return selectedSpawnpoint;

	}
}
