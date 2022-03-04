using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class VotePanelController : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		
	}

	public void VoteButtonClicked ()
	{
		if(!SettingPropetiesExtentions.GetPlayerIsAlive(PhotonNetwork.LocalPlayer.UserId)) return;


		string votedUserId = this.GetComponent<UserId>().userId;
		PlayerInfo votedPlayerInfo = GameInfomation.playerInfoDict[votedUserId];

		GameObject popup = this.transform.parent.parent.parent.parent.GetChild(3).gameObject;

		Text popupText = popup.transform.GetChild(0).GetComponent<Text>();
		popupText.text = "「" + votedPlayerInfo.nickname + "」に\n投票します。";

		VotePopupController votePopupController = popup.GetComponent<VotePopupController>();
		votePopupController.votedUserId = votedUserId;

		popup.SetActive(true);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
