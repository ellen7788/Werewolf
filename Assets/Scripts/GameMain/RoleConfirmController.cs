using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoleConfirmController : MonoBehaviour
{
	[SerializeField] GameObject NightCanvas;
	[SerializeField] GameObject AllCanvas;
	[SerializeField] GameObject roleText;
	[SerializeField] GameObject dayText;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = this.GetComponent<PhotonView>();
	}

	public void GameStartButtonClicked ()
	{
		photonView.RPC("StartNight", RpcTarget.All);
	}

	[PunRPC]
	void StartNight ()
	{
		PlayerInfo localPlayerInfo = GameInfomation.playerInfoDict[PhotonNetwork.LocalPlayer.UserId];
		roleText.GetComponent<Text>().text = "「" + localPlayerInfo.nickname + "」さんの役職は\n「" + localPlayerInfo.role.name_jp + "」です。";
		dayText.GetComponent<Text>().text = "1日目";

		AllCanvas.SetActive(true);
		NightCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
