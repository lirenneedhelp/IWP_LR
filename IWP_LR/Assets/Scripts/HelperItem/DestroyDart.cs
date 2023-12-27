using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DestroyDart : MonoBehaviourPun
{
    private Rigidbody rb;

    Vector3 targetPos;

    float shootForce = 20f;

    bool initialised = false;

    float height;

    Vector3 dir;

    Vector3 groundDirection;

    Vector3 firepos;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            dir = ray.direction;
            firepos = ray.origin;

            groundDirection = new Vector3(dir.x, 0, dir.z);

            targetPos = transform.position + dir * shootForce;
            height = targetPos.y + targetPos.magnitude / 2f;
            height = Mathf.Max(0.01f, height);
            Debug.Log(targetPos);
            Debug.Log(height);
        }
    }

    private void Update()
    {
        float angle;
        float v0;
        float time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

        if (!initialised)
        {
            StopAllCoroutines();
            StartCoroutine(Dart_Movement(groundDirection, v0, angle, time));
            initialised = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;

        if (photonView.IsMine)
            return;
        // Check if the collided object is a player
        PhotonView otherPhotonView = collision.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != null && otherPhotonView.IsMine)
        {
            // Store the GameObject and PhotonViewID before destroying the dart
            int targetViewID = otherPhotonView.ViewID;

            // Call RPC on the stored GameObject
            photonView.RPC(nameof(RPC_ShootDart), RpcTarget.AllViaServer, targetViewID);
        }     

    }

    [PunRPC]
    void RPC_ShootDart(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);

        if (targetPhotonView != null)
        {
            GameObject playerObject = targetPhotonView.gameObject;
            transform.SetParent(playerObject.transform);

            // Check if the target is the local player
            if (targetPhotonView.IsMine)
            {
                // Handle the impact on the target (e.g., remove a part, trigger an animation)
                Debug.Log("Dart hit the target!");

                // Get the PlayerController component from the target
                PlayerController pc = targetPhotonView.gameObject.GetComponent<PlayerController>();

                // Apply debuff using the PlayerController
                if (pc != null)
                {
                    Debug.Log("Applied Debuff");
                    pc.ApplyDebuff(5f);
                }
            }
        }

        // Destroy the dart on the shooter's client after RPC is completed
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DelayDartDestruction(10f));
        }

    }

    private IEnumerator DelayDartDestruction(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(photonView.gameObject);
    }

    IEnumerator Dart_Movement(Vector3 direction, float v0, float angle, float time)
    {
        float t = 0;

        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle);

            transform.position = firepos + direction * x + Vector3.up * y;
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void CalculatePath(Vector3 targetPos, float angle, out float v0, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float v1 = Mathf.Pow(xt, 2) * g;
        float v2 = 2 * xt * Mathf.Sin(angle) * Mathf.Cos(angle);
        float v3 = 2 * yt * Mathf.Pow(Mathf.Cos(angle), 2);

        v0 = Mathf.Sqrt(v1 / (v2 - v3));

        time = xt / (v0 * Mathf.Cos(angle));
    }

    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / xt);

        v0 = b / Mathf.Sin(angle);
    }

    private float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }


}
