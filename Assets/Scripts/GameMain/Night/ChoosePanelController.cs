using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChoosePanelController : MonoBehaviour
{
	string chosenUserId;
	PlayerInfo chosenPlayer;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	public void ChooseButtonClicked()
	{
		if(!GameInfomation.GetPlayerIsAlive(PhotonNetwork.LocalPlayer.UserId)) return;

		chosenUserId = this.GetComponent<UserId>().userId;

		GameObject popup = this.transform.parent.parent.parent.parent.GetChild(3).gameObject;
		ChoosePopupController popupController = popup.GetComponent<ChoosePopupController>();

		popupController.chosenUserId = chosenUserId;
		popup.SetActive(true);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
