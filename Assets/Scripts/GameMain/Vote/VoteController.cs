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
	string noneDeadUserId = "none";

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
		if(!SettingPropetiesExtentions.GetPlayerIsAlive(PhotonNetwork.LocalPlayer.UserId)) {
			votePlayerText.GetComponent<Text>().text = "死者は投票できません";
			voteButtonInteractable = false;
		} else {
			votePlayerText.GetComponent<Text>().text = "投票先を決めてください";
			voteButtonInteractable = true;
		}

		foreach (Player player in PhotonNetwork.PlayerList) {
			if (!SettingPropetiesExtentions.GetPlayerIsAlive(player.UserId)) continue;

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
			if (finVotePlayer.Count == SettingPropetiesExtentions.GetAlivingPlayerNum()) {
				string punishmentedUserId = GetPunishmentedUserId();
				SettingPropetiesExtentions.SetPlayerIsAlive(punishmentedUserId, false);

				string deadUserId = noneDeadUserId;
				PlayerInfo punishmentedPlayerInfo = GameInfomation.playerInfoDict[punishmentedUserId];
				Role punishmentedPlayerRole = punishmentedPlayerInfo.role;

				if(punishmentedPlayerRole.whenPunishmented == WhenPunishmented.punishmented) {}
				else if(punishmentedPlayerRole.whenPunishmented == WhenPunishmented.destinyBondAnyone){
					List<string> alivingPlayers = new List<string>();
					foreach(var playerInfo in GameInfomation.playerInfoDict) {
						if (SettingPropetiesExtentions.GetPlayerIsAlive(playerInfo.Key)) {
							alivingPlayers.Add(playerInfo.Key);
						}
					}

					deadUserId = alivingPlayers[Random.Range(0, alivingPlayers.Count)];
					SettingPropetiesExtentions.SetPlayerIsAlive(deadUserId, false);
				}
				else if(punishmentedPlayerRole.whenPunishmented == WhenPunishmented.destinyBondNotWolf){
					List<string> alivingNotWolfPlayers = new List<string>();
					foreach(var playerInfo in GameInfomation.playerInfoDict) {
						string playerId = playerInfo.Key;
						if (SettingPropetiesExtentions.GetPlayerIsAlive(playerId) && !GameInfomation.playerInfoDict[playerId].role.isWolf) {
							alivingNotWolfPlayers.Add(playerId);
						}
					}

					deadUserId = alivingNotWolfPlayers[Random.Range(0, alivingNotWolfPlayers.Count)];
					SettingPropetiesExtentions.SetPlayerIsAlive(deadUserId, false);
				}

				photonView.RPC("StartVoteResult", RpcTarget.All, punishmentedUserId, deadUserId);
			}
		}
	}

	[PunRPC]
	void StartVoteResult (string punishmentedUserId, string deadUserId) {
		finVotePlayer.Clear();

		Text punishmentUserText = voteResultCanvas.transform.GetChild(0).GetComponent<Text>();
		string punishmentUserNickname = GameInfomation.playerInfoDict[punishmentedUserId].nickname;
		punishmentUserText.text = "「" + punishmentUserNickname + "」さんが\n処刑されました";
		GameInfomation.addPunishmentedPlayer(punishmentedUserId);
		
		if(deadUserId != noneDeadUserId){
			string deadUserNickname = GameInfomation.playerInfoDict[deadUserId].nickname;
			punishmentUserText.text += "\n「" + deadUserNickname + "」さんが\n死亡しました。";
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

	// Update is called once per frame
	void Update()
	{
		
	}
}
