using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
	[SerializeField] TMP_InputField usernameInput;
	[SerializeField] int maxCharacterLimit = 10;

	void Start()
	{
		if(PlayerPrefs.HasKey("username"))
		{
			usernameInput.text = PlayerPrefs.GetString("username");
			PhotonNetwork.NickName = PlayerPrefs.GetString("username");
		}
		else
		{
			usernameInput.text = "Player " + Random.Range(0, 10000).ToString("0000");
			OnUsernameInputValueChanged();
		}

		// Add a listener to the OnValueChanged event of the InputField
		usernameInput.onValueChanged.AddListener(delegate { OnInputValueChanged(); });
	}

	public void OnUsernameInputValueChanged()
	{
		PhotonNetwork.NickName = usernameInput.text;
		PlayerPrefs.SetString("username", usernameInput.text);
	}

	void OnInputValueChanged()
	{
		// Check if the input length exceeds the maximum limit
		if (usernameInput.text.Length > maxCharacterLimit)
		{
			// Trim the text to the maximum limit
			usernameInput.text = usernameInput.text.Substring(0, maxCharacterLimit);
		}
	}
}
