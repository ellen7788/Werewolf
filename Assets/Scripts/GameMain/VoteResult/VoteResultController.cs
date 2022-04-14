using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class VoteResultController : MonoBehaviour
{
	[SerializeField] GameObject NightCanvas;
	[SerializeField] Text punishmentUserText;
	[SerializeField] Text voteResultText;
	[SerializeField] Text voteInfoText;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = GetComponent<PhotonView>();
		voteInfoText.gameObject.SetActive(GameInfomation.gameSetting.displayVoteInfo);
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

		SetVoteResult();
	}

	void SetVoteResult()
	{
		voteResultText.text = "";
		voteInfoText.text = "";

		Dictionary<string, string> voteInfo = GameInfomation.GetVoteInfo();
		Dictionary<string, int> voteCount = new Dictionary<string, int>();

		foreach (KeyValuePair<string, string> item in voteInfo) {
			if(voteCount.ContainsKey(item.Value)) voteCount[item.Value]++;
			else voteCount.Add(item.Value, 1);

			voteInfoText.text += "「" + GameInfomation.playerInfoDict[item.Key].nickname + "」→「" + GameInfomation.playerInfoDict[item.Value].nickname + "」\n";
		}
		var orderdvoteCount = voteCount.OrderByDescending((x) => x.Value);

		foreach(var votedIdAndNum in orderdvoteCount){
			string id = votedIdAndNum.Key;
			int num = votedIdAndNum.Value;

			voteResultText.text += "「" + GameInfomation.playerInfoDict[id].nickname + "」→" + num + "票\n";
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
