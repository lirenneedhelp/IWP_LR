using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private List<Spawnpoint> availableSpawnpoints = new List<Spawnpoint>();

    void Awake()
    {
        Instance = this;
        PopulateSpawnpointsList();
    }

    void PopulateSpawnpointsList()
    {
        // Get all Spawnpoint components in the children
        Spawnpoint[] spawnpoints = GetComponentsInChildren<Spawnpoint>();

        // Add all spawnpoints to the availableSpawnpoints list
        availableSpawnpoints.AddRange(spawnpoints);
    }

    public Transform GetSpawnpoint()
    {
        if (availableSpawnpoints.Count == 0)
        {
            // No available spawn points, handle this case (e.g., respawn logic or return null)
            Debug.LogWarning("No available spawn points!");
            return null;
        }

        // Choose a random spawn point from the availableSpawnpoints list
        int randomIndex = Random.Range(0, availableSpawnpoints.Count);
        Spawnpoint selectedSpawnpoint = availableSpawnpoints[randomIndex];

        // Remove the selected spawn point from the availableSpawnpoints list
        availableSpawnpoints.RemoveAt(randomIndex);

        // Return the selected spawn point's transform
        return selectedSpawnpoint.transform;
    }
}
