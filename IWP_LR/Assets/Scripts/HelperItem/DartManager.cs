using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DartManager : MonoBehaviour
{
    public static DartManager Instance = null;

    public float destroyDelay = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject Instantiate(Vector3 pos, Quaternion rot)
    {
        GameObject dart = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Dart"), pos, rot);
        // After a delay, destroy the dart across the network
        StartCoroutine(DestroyDartDelayed(dart));

        return dart;
    }

    IEnumerator DestroyDartDelayed(GameObject dart)
    {
        yield return new WaitForSeconds(destroyDelay);

        // Ensure the dart still exists before attempting to destroy it
        if (dart != null && dart.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(dart);
        }
    }
}
