using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VoteController : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject PlayerVotePanel;
	[SerializeField] GameObject content;
	[SerializeField] GameObject voteResultCanvas;
	[SerializeField] GameObject votePlayerText;
	[SerializeField] GameObject votePopup;
	Dictionary<string, string> finVotePlayer;
	bool started = false;

	// Start is called before the first frame update
	void Start()
	{
		SetPlayerVotePanel();
		finVotePlayer = new Dictionary<string, string>();
		started = true;
	}

	public override void OnEnable()
	{
		base.OnEnable();

		if(!started) return;
		SetPlayerVotePanel();
	}

	void SetPlayerVotePanel(){
		foreach (Transform child in content.transform) {
			Destroy(child.gameObject);
		}

		votePopup.SetActive(false);

		bool voteButtonInteractable;
		if(!GameInfomation.GetPlayerIsAlive(PhotonNetwork.LocalPlayer.UserId)) {
			votePlayerText.GetComponent<Text>().text = "死者は投票できません";
			voteButtonInteractable = false;
		} else {
			votePlayerText.GetComponent<Text>().text = "投票先を決めてください";
			voteButtonInteractable = true;
		}

		foreach (Player player in PhotonNetwork.PlayerList) {
			if (!GameInfomation.GetPlayerIsAlive(player.UserId)) continue;

			GameObject newPlayerVotePanel = Instantiate(PlayerVotePanel, Vector3.zero, Quaternion.identity);
			Text playerName = newPlayerVotePanel.transform.GetChild(0).GetComponent<Text>();
			playerName.text = player.NickName;

			UserId userId = newPlayerVotePanel.GetComponent<UserId>();
			userId.userId = player.UserId;

			Button voteButton = newPlayerVotePanel.transform.GetChild(1).GetComponent<Button>();
			voteButton.interactable = voteButtonInteractable;

			newPlayerVotePanel.transform.SetParent(content.transform);
		}
	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps) {
		foreach (DictionaryEntry de in changedProps) {
			if (de.Key.ToString().Split('.')[1] != SettingPropetiesExtentions.voteToken) return;

			string key = de.Key.ToString().Split('.')[0];
			string value = de.Value.ToString();

			if (finVotePlayer.ContainsKey(key)) finVotePlayer[key] = value;
			else finVotePlayer.Add(key, value);
		}

		if (PhotonNetwork.IsMasterClient) {
			if (finVotePlayer.Count == GameInfomation.GetAlivingPlayerNum()) {
				string punishmentedUserId = GetPunishmentedUserId();

				List<string> deadUsersIdList = new List<string>() { "dummy", "dummy" };
				deadUsersIdList.AddRange(GetDeadUsersId(punishmentedUserId));
				string[] deadUsersId = deadUsersIdList.ToArray();

				photonView.RPC("StartVoteResult", RpcTarget.All, punishmentedUserId, deadUsersId);
			}
		}
	}

	[PunRPC]
	void StartVoteResult (string punishmentedUserId, string[] deadUsersIdArray) {
		// dummyを取り除く
		List<string> deadUsersId = deadUsersIdArray.ToList();
		deadUsersId.RemoveRange(0, 2);

		GameInfomation.SetVoteInfo(finVotePlayer);
		finVotePlayer.Clear();

		Text punishmentUserText = voteResultCanvas.transform.GetChild(0).GetComponent<Text>();
		string punishmentUserNickname = GameInfomation.playerInfoDict[punishmentedUserId].nickname;
		punishmentUserText.text = "「" + punishmentUserNickname + "」さんが\n処刑されました";
		GameInfomation.SetPunishmentedPlayerId(punishmentedUserId);

		if(deadUsersId.Count > 0){
			punishmentUserText.text += "\n";
			foreach(string deadUserId in deadUsersId){
				string deadUserNickname = GameInfomation.playerInfoDict[deadUserId].nickname;
				punishmentUserText.text += "「" + deadUserNickname + "」";
			}
			punishmentUserText.text += "さんが\n死亡しました。";
			GameInfomation.SetDestinyBondedPlayerId(deadUsersId);
		}

		voteResultCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}

	string GetPunishmentedUserId() {
		Dictionary<string, int> voteResult = new Dictionary<string, int>();

		foreach (KeyValuePair<string, string> item in finVotePlayer) {
			if(voteResult.ContainsKey(item.Value)) voteResult[item.Value]++;
			else voteResult.Add(item.Value, 1);
		}

		int maxVotedCount = voteResult.Values.Max();
		List<KeyValuePair<string, int>> maxVotedUsers = voteResult.Where(c => c.Value == maxVotedCount).ToList();
		KeyValuePair<string, int> punishmentedUser = maxVotedUsers[Random.Range(0, maxVotedUsers.Count)];

		return punishmentedUser.Key;
	}

	List<string> GetDeadUsersId(string punishmentedUserId) {
		HashSet<string> deadUsersId = new HashSet<string>();

		PlayerInfo punishmentedPlayerInfo = GameInfomation.playerInfoDict[punishmentedUserId];
		Role punishmentedPlayerRole = punishmentedPlayerInfo.role;

		if(punishmentedPlayerRole.whenPunishmented == WhenPunishmented.punishmented) {}
		else if(punishmentedPlayerRole.whenPunishmented == WhenPunishmented.destinyBondAnyone){
			List<string> alivingPlayers = new List<string>();
			foreach(var playerInfo in GameInfomation.playerInfoDict) {
				string playerId = playerInfo.Key;
				if (GameInfomation.GetPlayerIsAlive(playerInfo.Key) && playerId != PhotonNetwork.LocalPlayer.UserId) {
					alivingPlayers.Add(playerInfo.Key);
				}
			}

			string deadUserId = alivingPlayers[Random.Range(0, alivingPlayers.Count)];
			deadUsersId.Add(deadUserId);
		}
		else if(punishmentedPlayerRole.whenPunishmented == WhenPunishmented.destinyBondNotWolf){
			List<string> alivingNotWolfPlayers = new List<string>();
			foreach(var playerInfo in GameInfomation.playerInfoDict) {
				string playerId = playerInfo.Key;
				if (GameInfomation.GetPlayerIsAlive(playerId) && !GameInfomation.playerInfoDict[playerId].role.isWolf && playerId != PhotonNetwork.LocalPlayer.UserId) {
					alivingNotWolfPlayers.Add(playerId);
				}
			}

			string deadUserId = alivingNotWolfPlayers[Random.Range(0, alivingNotWolfPlayers.Count)];
			deadUsersId.Add(deadUserId);
		}

		// 後追い
		foreach(var playerData in GameInfomation.playerInfoDict){
			string playerId = playerData.Key;
			Chase playerChase = playerData.Value.role.chase;

			if(playerChase == Chase.none){}
			else if(playerChase == Chase.fox){
				foreach(var deadPlayerId in deadUsersId){
					if(GameInfomation.playerInfoDict[deadPlayerId].role.isFox){
						deadUsersId.Add(playerId);
						break;
					}
				}

				if(GameInfomation.playerInfoDict[punishmentedUserId].role.isFox){
					deadUsersId.Add(playerId);
				}
			}
		}

		return deadUsersId.ToList();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
