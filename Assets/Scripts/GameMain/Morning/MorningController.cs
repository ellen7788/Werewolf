using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MorningController : MonoBehaviour
{
	[SerializeField] GameObject DaytimeCanvas;
	[SerializeField] GameObject DayText;
	[SerializeField] Text morningText;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = GetComponent<PhotonView>();
	}

	void OnEnable(){
		DayText.GetComponent<Text>().text = GameInfomation.day + "日目";

		List<string> deadUsersId = GameInfomation.GetDeadPlayersId();
		string message = "";
		if(deadUsersId.Count <= 0) { 
			message += "朝になりました。今日の犠牲者は\nいませんでした。";
		}
		else {
			message += "朝になりました。今日の犠牲者は\n";
			for (int i = 0; i < deadUsersId.Count; i++){
				message += "「" + GameInfomation.playerInfoDict[deadUsersId[i]].nickname + "」";
			}
			message += "です。";
		}
		morningText.text = message;

		// 朝行動
		foreach(var playerInfo in GameInfomation.playerInfoDict) {
			string playerId = playerInfo.Key;
			MorningAction morningAction = playerInfo.Value.role.morningAction;

			if(morningAction == MorningAction.none){}
			else if(morningAction == MorningAction.deliveryBread){
				if(GameInfomation.playerInfoDict[playerId].isAlive) morningText.text += "\nおいしいパンが届けられました";
			}
		}
	}

	public void NextButtonClicked()
	{
		photonView.RPC("StartDaytime", RpcTarget.All);
	}

	[PunRPC]
	void StartDaytime() {
		// 勝利条件に当てはまったときはリザルト画面に移る
		if(GameInfomation.judgeGameEnd()) {
			string sceneName = "Result";
			PhotonNetwork.LoadLevel(sceneName);
		}
		else {
			DaytimeCanvas.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}


	// Update is called once per frame
	void Update()
	{
		
	}
}
