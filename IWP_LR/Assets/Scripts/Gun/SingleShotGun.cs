﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	float maxRaycastDistance = 5f; // Set your desired maximum raycast distance here


	PhotonView PV;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	public override void Use()
	{
		Shoot();
	}

	void Shoot()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;
		if(Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
		{
			Vector3 knockbackDirection = (hit.point - transform.position).normalized;
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage, knockbackDirection);
			PV.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
		}
	}

	[PunRPC]
	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
		if(colliders.Length != 0)
		{
			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
			Destroy(bulletImpactObj, 0.14f);
			bulletImpactObj.transform.SetParent(colliders[0].transform);
		}
	}

}
