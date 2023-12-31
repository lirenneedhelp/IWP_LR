using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DartManager : MonoBehaviour
{
    public static DartManager Instance = null;

    public float destroyDelay = 10f;

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
    public GameObject InstantiateDart(Vector3 pos, Quaternion rot)
    {
        //rot.y += -90;
        GameObject dart = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "DartFinished"), pos, rot);
        Vector3 offset = new(0, -90, 0);
        dart.transform.localEulerAngles += offset;
        // After a delay, destroy the dart across the network
        //StartCoroutine(DestroyDartDelayed(dart));

        return dart;
    }

    public IEnumerator DestroyDartDelayed(GameObject dart)
    {
        yield return new WaitForSeconds(destroyDelay);

        // Ensure the dart still exists before attempting to destroy it
        if (dart != null && dart.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(dart);
        }
    }
}
