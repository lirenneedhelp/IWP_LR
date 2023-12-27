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
	public Camera cam;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime, taggerSpeedMultiplier;

	public InventoryManager inventoryManager;

	[SerializeField] Item fist;

	int itemIndex = 0;
	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	//const float maxHealth = 100f;

	const int knockbackForce = 5;

	//float currentHealth = maxHealth;

	public PlayerManager playerManager;

	private float ticksSinceLastAttack = 0f;
	private float attackCooldown = 0.5f; // Change this to set the desired attack cooldown time

	public Animator animator;

	float cacheWalkSpeed, cacheSprintSpeed, cacheJumpForce;

	private Vector3 lastPosition;
	private float lastTime;

	private Vector3 currentVelocity;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		animator = GetComponent<Animator>();

		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		//ToggleMouse.OffCursor();

		cacheWalkSpeed = walkSpeed;
		cacheSprintSpeed = sprintSpeed;
		cacheJumpForce = jumpForce;
	}

	void Start()
	{
		if(PV.IsMine)
		{
			EquipItem(0);
			lastPosition = rb.position;
			lastTime = Time.time;
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

		if (playerManager.isTyping)
			moveAmount = Vector3.zero;

		if (playerManager.isTyping == false)
		{
			Look();
			Move();
			Jump();


			// Increment ticks since last attack
			ticksSinceLastAttack += Time.deltaTime;

			for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
			{
				if (Input.GetKeyDown((i + 1).ToString()))
				{
					EquipItem(i);
					break;
				}
			}

			if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
			{
				if (itemIndex >= inventoryManager.inventorySlots.Length - 1)
				{
					EquipItem(0);
				}
				else
				{
					EquipItem(itemIndex + 1);
				}
			}
			else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
			{
				if (itemIndex <= 0)
				{
					EquipItem(inventoryManager.inventorySlots.Length - 1);
				}
				else
				{
					EquipItem(itemIndex - 1);
				}
			}


			// CODE FOR SHOOTING!
			if (Input.GetMouseButtonDown(0) && ticksSinceLastAttack >= attackCooldown)
			{
				// Reset ticks since last attack
				ticksSinceLastAttack = 0f;
				animator.SetTrigger("IsAttack");
				fist.Use();
				return;
			}
			else if (Input.GetMouseButtonDown(1))
            {
				if (inventoryManager.inventorySlots[itemIndex].item != null)
				{
					//Debug.Log(items[itemIndex].itemInfo.quantity);
					//if (inventoryManager.inventorySlots[itemIndex].item.itemInfo.quantity > 0)
					Debug.Log("Using Item");
					inventoryManager.inventorySlots[itemIndex].item.Item.Use();
					inventoryManager.inventorySlots[itemIndex].item.count--;
					inventoryManager.inventorySlots[itemIndex].item.RefreshCount();
				}
			}
		}
		

		if(transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}
	}

	void Look()
	{
		transform.Rotate(Input.GetAxisRaw("Mouse X") * mouseSensitivity * Vector3.up);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{

		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");

		//Debug.Log("Horizontal:" + x + "Vertical:" + z);

		float move = Mathf.Abs(x) + Mathf.Abs(z); 
		float speedMultiplier = playerManager.isTagger ? taggerSpeedMultiplier : 1f;

		bool IsSprint = Input.GetKey(KeyCode.LeftShift);
		
		animator.SetBool("IsWalking", move > 0);
		animator.SetBool("IsSprinting", IsSprint);

		Vector3 moveDir = new Vector3(x, 0, z).normalized;
		moveAmount = Vector3.SmoothDamp(moveAmount, (IsSprint ? sprintSpeed : walkSpeed) * speedMultiplier * moveDir, ref smoothMoveVelocity, smoothTime);

		// Calculate displacement and time difference
		Vector3 displacement = rb.position - lastPosition;
		float deltaTime = Time.time - lastTime;

		// Calculate velocity
		currentVelocity = displacement / deltaTime;

		// Update last position and time for the next frame
		lastPosition = rb.position;
		lastTime = Time.time;

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
		//if (inventoryManager.inventorySlots[itemIndex].item == null)
			//return;
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		if (PV.IsMine)
		{
			if (previousItemIndex != -1)
				inventoryManager.inventorySlots[previousItemIndex].Deselect();

			inventoryManager.inventorySlots[itemIndex].Selected();
		}
		//inventoryManager.inventorySlots[itemIndex].item.itemGameObject.SetActive(true);

		if (previousItemIndex != -1)
		{
			//inventoryManager.inventorySlots[previousItemIndex].item.itemGameObject.SetActive(false);
		}
		previousItemIndex = itemIndex;

		

		if (PV.IsMine)
		{
			Hashtable hash = new ();
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

	public void TakeDamage(float damage, Vector3 dir)
	{
		//PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
		ApplyKnockback(dir);
		PV.RPC(nameof(RPC_SwapTagger), RpcTarget.AllViaServer);
	}
	public void ApplyKnockback(Vector3 knockbackDirection)
	{
		// Calculate knockback velocity using current velocity and additional knockback force
		Vector3 knockbackVelocity = knockbackDirection.normalized * knockbackForce;
		knockbackVelocity.y += 0.5f;
		PV.RPC("ApplyKnockbackRPC", PV.Owner, knockbackVelocity);

	}

	[PunRPC]
    private void ApplyKnockbackRPC(Vector3 knockbackVelocity, PhotonMessageInfo info)
    {
		// Apply the received knockback velocity to the rigidbody for remote players
		var totalKnockBackVelocity = knockbackVelocity + info.photonView.gameObject.GetComponent<PlayerController>().currentVelocity;
		rb.velocity = totalKnockBackVelocity;
    }

	[PunRPC]
	void RPC_SwapTagger(PhotonMessageInfo info)
	{
		PlayerManager sender = PlayerManager.Find(info.Sender);
		//Debug.LogError("Sender:" + sender.PV.Owner);
		//Debug.LogError("Receiver:" + PV.Owner);
		
		if (!playerManager.isTagger && sender.isTagger)
		{
			//Debug.Log(info.Sender + " tagged " + PV.Owner);
			sender.isTagger = false;
			playerManager.isTagger = true;
			EventManager.AnnounceTaggedPlayer(PV.ViewID);	
			//Debug.LogError(playerManager.isTagger);
		}
	}
	//[PunRPC]
	//void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	//{
	//	currentHealth -= damage;

	//	healthbarImage.fillAmount = currentHealth / maxHealth;

	//	if(currentHealth <= 0)
	//	{
	//		Die();
	//		PlayerManager.Find(info.Sender).GetKill();
	//	}
	//}

	public void ApplyDebuff(float debuffDuration)
    {
		sprintSpeed *= 0.5f;
		walkSpeed *= 0.5f;

		StartCoroutine(RevertSpeedAfterDelay(debuffDuration));
	}

	private IEnumerator RevertSpeedAfterDelay(float delay)
	{
		// Wait for the specified delay
		yield return new WaitForSeconds(delay);

		// Revert the speed to the original values
		ClearDebuff();
	}

	private void ClearDebuff()
    {
		walkSpeed = cacheWalkSpeed;
		sprintSpeed = cacheSprintSpeed;
    }

	public void ApplyJumpBoost(float boost)
    {
		jumpForce *= boost;

		StartCoroutine(RevertJumpForceAfterDelay(10f));
    }

	private IEnumerator RevertJumpForceAfterDelay(float duration)
    {
		yield return new WaitForSeconds(duration);

		ClearJumpBuff();
    }

	private void ClearJumpBuff()
    {
		jumpForce = cacheJumpForce;
    }

	public void ApplySpeed(float speed_boost)
    {
		walkSpeed *= speed_boost;
		sprintSpeed *= speed_boost;

		StartCoroutine(RevertSpeedAfterDelay(10f));

	}


}