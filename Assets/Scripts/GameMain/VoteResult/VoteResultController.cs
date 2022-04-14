using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class VoteResultController : MonoBehaviour
{
	[SerializeField] GameObject NightCanvas;
	[SerializeField] Text punishmentUserText;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = GetComponent<PhotonView>();
	}

	void OnEnable()
	{
		string punishmentedUserId = GameInfomation.GetPunishmentedPlayerId();
		string punishmentUserNickname = GameInfomation.playerInfoDict[punishmentedUserId].nickname;
		punishmentUserText.text = "「" + punishmentUserNickname + "」さんが\n処刑されました";

		List<string> deadUsersId = GameInfomation.GetDestinyBondedPlayerId();
		if(deadUsersId.Count > 0){
			punishmentUserText.text += "\n";
			foreach(string deadUserId in deadUsersId){
				string deadUserNickname = GameInfomation.playerInfoDict[deadUserId].nickname;
				punishmentUserText.text += "「" + deadUserNickname + "」";
			}
			punishmentUserText.text += "さんが\n死亡しました。";
		}
	}

	public void NextButtonClicked()
	{
		photonView.RPC("StartNight", RpcTarget.All);
	}

	[PunRPC]
	void StartNight() {
		// 勝利条件に当てはまったときはリザルト画面に移る
		if(GameInfomation.judgeGameEnd()) {
			string sceneName = "Result";
			PhotonNetwork.LoadLevel(sceneName);
		}
		else {
			NightCanvas.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
