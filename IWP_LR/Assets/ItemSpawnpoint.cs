using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnpoint : MonoBehaviour
{
	[SerializeField] MeshRenderer graphics;
	public GameObject waypoint;

	void Awake()
	{
		graphics.enabled = false;
	}
}
