using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
	[SerializeField] Image healthbarImage;
	[SerializeField] GameObject ui;

	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime, taggerSpeedMultiplier;

	[SerializeField] Item[] items;

	int itemIndex;
	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;

	const int knockbackForce = 5;

	float currentHealth = maxHealth;

	PlayerManager playerManager;

	private float ticksSinceLastAttack = 0f;
	private float attackCooldown = 0.5f; // Change this to set the desired attack cooldown time

	public Animator animator;


	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		animator = GetComponent<Animator>();

		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		ToggleMouse.OffCursor();
	}

	void Start()
	{
		if(PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			Destroy(ui);
		}
	}

	void Update()
	{
		if(!PV.IsMine)
			return;

		Look();
		Move();
		Jump();
		
		// Increment ticks since last attack
    	ticksSinceLastAttack += Time.deltaTime;

		for(int i = 0; i < items.Length; i++)
		{
			if(Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}

		if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if(itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if(itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}
		

		// CODE FOR SHOOTING!
		if(Input.GetMouseButtonDown(0) && ticksSinceLastAttack >= attackCooldown)
		{
			// Reset ticks since last attack
			ticksSinceLastAttack = 0f;
			items[itemIndex].Use();
		}

		if (Input.GetMouseButtonDown(0))
		{
			animator.SetTrigger("IsAttack");
		}

		if(transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}
	}

	void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");

		float move = x + z;
		float speedMultiplier = playerManager.isTagger ? taggerSpeedMultiplier : 1f;

		bool IsSprint = Input.GetKey(KeyCode.LeftShift);
		
		animator.SetBool("IsWalking", move != 0);
		animator.SetBool("IsSprinting", IsSprint);

		Vector3 moveDir = new Vector3(x, 0, z).normalized;
		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (IsSprint ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
		
	}

	void Jump()
	{
		if(Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
			animator.SetBool("IsJump", true);
		}
	}

	void EquipItem(int _index)
	{
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if(PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if(!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	void Die()
	{
		playerManager.Die();
	}

	public void ApplyKnockback(Vector3 knockbackDirection)
    {
        // Calculate knockback velocity using current velocity and additional knockback force
        Vector3 knockbackVelocity = knockbackDirection.normalized * knockbackForce;
		knockbackVelocity.y += 0.5f;
        PV.RPC("ApplyKnockbackRPC", PV.Owner, knockbackVelocity);
        
    }

	public void TakeDamage(float damage, Vector3 dir)
	{
		//PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
		ApplyKnockback(dir);
		PV.RPC(nameof(RPC_SwapTagger), RpcTarget.All);
	}

	[PunRPC]
	void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	{
		currentHealth -= damage;

		healthbarImage.fillAmount = currentHealth / maxHealth;

		if(currentHealth <= 0)
		{
			Die();
			PlayerManager.Find(info.Sender).GetKill();
		}
	}

	[PunRPC]
    private void ApplyKnockbackRPC(Vector3 knockbackVelocity)
    {
        // Apply the received knockback velocity to the rigidbody for remote players
        rb.velocity = knockbackVelocity;
    }

	[PunRPC]
	void RPC_SwapTagger(PhotonMessageInfo info)
	{
		PlayerManager sender = PlayerManager.Find(info.Sender);
		//Debug.LogError("Sender:" + sender.PV.Owner);
		//Debug.LogError("Receiver:" + PV.Owner);
		
		if (sender.isTagger != playerManager.isTagger && sender.isTagger)
		{
			//Debug.Log(info.Sender + " tagged " + PV.Owner);
			sender.SwapTagger(false);
			playerManager.isTagger = true;
			EventManager.AnnounceTaggedPlayer(PV.ViewID);	
			//Debug.LogError(playerManager.isTagger);
		}
	}

}