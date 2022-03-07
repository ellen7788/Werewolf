using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NightController : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject chooseText;
	[SerializeField] GameObject choosePanel;
	[SerializeField] GameObject content;
	[SerializeField] GameObject popup;
	[SerializeField] GameObject MorningCanvas;
	Dictionary<string, string> finChoosePlayer;
	bool started = false;

	// Start is called before the first frame update
	void Start()
	{
		SetChoosePanel();
		finChoosePlayer = new Dictionary<string, string>();
		started = true;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (!started) return;

		SetChoosePanel();
	}

	void SetChoosePanel(){
		foreach (Transform child in content.transform) {
			Destroy(child.gameObject);
		}
		popup.SetActive(false);

		// TODO 霊媒結果が表示されない(霊媒結果を表示するのは処刑された=死んだ人のため)
		Role myRole = GameInfomation.playerInfoDict[PhotonNetwork.LocalPlayer.UserId].role;

		bool chooseButtonInteractable;
		if(!SettingPropetiesExtentions.GetPlayerIsAlive(PhotonNetwork.LocalPlayer.UserId)) {
			chooseText.GetComponent<Text>().text = "死者は選択できません";
			chooseButtonInteractable = false;
		} else {
			chooseText.GetComponent<Text>().text = GetTitle();
			chooseButtonInteractable = true;
		}

		foreach (Player player in PhotonNetwork.PlayerList) {
			GameObject newPlayerVotePanel = Instantiate(choosePanel, Vector3.zero, Quaternion.identity);
			Text playerName = newPlayerVotePanel.transform.GetChild(0).GetComponent<Text>();
			playerName.text = player.NickName;

			UserId userId = newPlayerVotePanel.GetComponent<UserId>();
			userId.userId = player.UserId;

			Button chooseButton = newPlayerVotePanel.transform.GetChild(1).GetComponent<Button>();
			chooseButton.interactable = chooseButtonInteractable;

			Role playerRole = GameInfomation.playerInfoDict[player.UserId].role;

			// TODO 役職に応じて表示変更、role_name直打ちになっているので何とかしたい
			if(myRole.role_name == "werewolf"){
				if(playerRole.role_name == "werewolf") {
					playerName.text += "\n<color=red>人狼</color>";
					chooseButton.interactable = false;
				}
			}
			else if(myRole.role_name == "fotuneTeller"){
				if(player.UserId == PhotonNetwork.LocalPlayer.UserId) chooseButton.interactable = false;
			}
			else if(myRole.role_name == "medium"){
				if(GameInfomation.punishmentedPlayersId.Count > 0 && player.UserId == GameInfomation.punishmentedPlayersId[GameInfomation.punishmentedPlayersId.Count-1]){
					if(playerRole.mediumRes == MediumRes.citizen) playerName.text += "\n人狼ではない";
					else if(playerRole.mediumRes == MediumRes.wolf) playerName.text += "\n<color=red>人狼</color>";
				}
			}
			else if(myRole.role_name == "knight"){
				if(player.UserId == PhotonNetwork.LocalPlayer.UserId) chooseButton.interactable = false;
			}

			if (!SettingPropetiesExtentions.GetPlayerIsAlive(player.UserId)) {
				playerName.text += "\n<color=red>死亡</color>";
				chooseButton.interactable = false;
			}

			newPlayerVotePanel.transform.SetParent(content.transform);
		}
	}

	string GetTitle() {
		string title = "";
		NightAction myNightAction = GameInfomation.playerInfoDict[PhotonNetwork.LocalPlayer.UserId].role.nightAction;

		if(myNightAction == NightAction.suspectWolf) title = "疑わしい人を選んでください";
		else if(myNightAction == NightAction.biteToKill) {
			if(GameInfomation.day == 1) title = "初日は噛めないけど誰か選んどいてください";
			else title = "噛み先を選んでください";
		}
		else if(myNightAction == NightAction.fotuneTelling) title = "占い先を選んでください";
		else if(myNightAction == NightAction.seeMediumRes) title = "疑わしい人を選んでください";
		else if(myNightAction == NightAction.guardOtherPeople) title = "護衛先を選んでください";
		else title = "疑わしい人を選んでください";

		return title;
	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps) {
		foreach (DictionaryEntry de in changedProps) {
			if (de.Key.ToString().Split('.')[1] != SettingPropetiesExtentions.chooseToken) return;

			string choosingUserId = de.Key.ToString().Split('.')[0];
			string chosenUserId = de.Value.ToString();

			if (finChoosePlayer.ContainsKey(choosingUserId)) finChoosePlayer[choosingUserId] = chosenUserId;
			else finChoosePlayer.Add(choosingUserId, chosenUserId);
		}

		if (PhotonNetwork.IsMasterClient) {
			if (finChoosePlayer.Count == SettingPropetiesExtentions.GetAlivingPlayerNum()) {
				string[] deadPlayersUserId = GetDeadPlayersUserId();

				for(int i = 0; i < deadPlayersUserId.Length; i++){
					SettingPropetiesExtentions.SetPlayerIsAlive(deadPlayersUserId[i], false);
				}

				if(deadPlayersUserId.Length == 0) photonView.RPC("StartMorningWithNoneDeadPlayer", RpcTarget.All);
				else if(deadPlayersUserId.Length == 1) photonView.RPC("StartMorningWithOneDeadPlayer", RpcTarget.All, deadPlayersUserId[0]);
				else photonView.RPC("StartMorningWithMultipleDeadPlayer", RpcTarget.All, deadPlayersUserId);
			}
		}
	}

	[PunRPC]
	void StartMorningWithNoneDeadPlayer(){
		finChoosePlayer.Clear();

		Text morningText = MorningCanvas.transform.GetChild(0).GetComponent<Text>();
		morningText.text = "朝になりました。今日の犠牲者は\nいませんでした。";

		GameInfomation.day++;

		MorningCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}

	[PunRPC]
	void StartMorningWithOneDeadPlayer(string deadUserId){
		finChoosePlayer.Clear();

		Text morningText = MorningCanvas.transform.GetChild(0).GetComponent<Text>();
		morningText.text = "朝になりました。今日の犠牲者は\n「" + GameInfomation.playerInfoDict[deadUserId].nickname + "」です。";

		GameInfomation.day++;

		MorningCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}

	[PunRPC]
	void StartMorningWithMultipleDeadPlayer(string[] deadUsersId) {
		finChoosePlayer.Clear();

		Text morningText = MorningCanvas.transform.GetChild(0).GetComponent<Text>();

		morningText.text = "朝になりました。今日の犠牲者は\n「";
		morningText.text += GameInfomation.playerInfoDict[deadUsersId[0]].nickname;
		for (int i = 1; i < deadUsersId.Length; i++){
			morningText.text += "、" + GameInfomation.playerInfoDict[deadUsersId[i]].nickname;
		}
		morningText.text += "」です。";

		GameInfomation.day++;

		MorningCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}

	string[] GetDeadPlayersUserId(){
		// 夜の出来事の結果を処理し、死者を求める。
		HashSet<string> deadPlayersUserId = new HashSet<string>();

		// 役職行動によるステータスフラグをかける
		Dictionary<string, StatusNight> playerStatusDict = new Dictionary<string, StatusNight>();
		List<(string, string)> bitedAndBitePlayersId = new List<(string, string)>();
		foreach (KeyValuePair<string, string> chooseInfo in finChoosePlayer){
			string fromUserId = chooseInfo.Key;
			string toUserId = chooseInfo.Value;

			PlayerInfo fromPlayer = GameInfomation.playerInfoDict[fromUserId];
			PlayerInfo toPlayer   = GameInfomation.playerInfoDict[toUserId];

			NightAction fromPlayerNightAction = fromPlayer.role.nightAction;
			if(!playerStatusDict.ContainsKey(toUserId)) playerStatusDict.Add(toUserId, new StatusNight());

			if(fromPlayerNightAction == NightAction.suspectWolf) {}
			else if(fromPlayerNightAction == NightAction.biteToKill) {
				// 初日は噛めない
				if(GameInfomation.day != 1) bitedAndBitePlayersId.Add((toUserId, fromUserId));
			}
			else if(fromPlayerNightAction == NightAction.fotuneTelling) {
				playerStatusDict[toUserId].PlayerIdFotuneTellMe = fromUserId;
			}
			else if(fromPlayerNightAction == NightAction.seeMediumRes) {}
			else if(fromPlayerNightAction == NightAction.guardOtherPeople) {
				playerStatusDict[toUserId].PlayerIdGuradMe = fromUserId;
			}
		}

		// 人狼の噛み先が複数いたら、ランダムに1人選ぶ
		if(bitedAndBitePlayersId.Count > 1){
			int bitedPlayerIndex = Random.Range(0, bitedAndBitePlayersId.Count);
			(string bitedPlayerId, string bitePlayerId) = bitedAndBitePlayersId[bitedPlayerIndex];
			playerStatusDict[bitedPlayerId].PlayerIdBiteMe = bitePlayerId;
		}else if(bitedAndBitePlayersId.Count == 1){
			(string bitedPlayerId, string bitePlayerId) = bitedAndBitePlayersId[0];
			playerStatusDict[bitedPlayerId].PlayerIdBiteMe = bitePlayerId;
		}

		// ステータスフラグから死者を求める
		foreach (KeyValuePair<string, StatusNight> playerStatus in playerStatusDict) {
			string playerId = playerStatus.Key;
			PlayerInfo playerInfo = GameInfomation.playerInfoDict[playerId];
			Role playerRole = playerInfo.role;
			StatusNight status = playerStatus.Value;

			if(status.PlayerIdBiteMe != null){
				if(status.PlayerIdGuradMe != null) {}

				else if(playerRole.whenBited == WhenBited.death) { deadPlayersUserId.Add(playerId); }
				else if(playerRole.whenBited == WhenBited.cannotBite) {}
				else if(playerRole.whenBited == WhenBited.notDeath) {}
				else if(playerRole.whenBited == WhenBited.killWolf) { deadPlayersUserId.Add(status.PlayerIdBiteMe); }
			}

			if(status.PlayerIdFotuneTellMe != null){
				if(playerRole.whenFortuneTelled == WhenFortuneTelled.none) {}
				else if(playerRole.whenFortuneTelled == WhenFortuneTelled.death) { deadPlayersUserId.Add(playerId); }
				else if(playerRole.whenFortuneTelled == WhenFortuneTelled.killFortuneTeller) { deadPlayersUserId.Add(status.PlayerIdFotuneTellMe); }
			}

			if(status.PlayerIdGuradMe != null){
				if(playerRole.whenGuarded == WhenGuarded.guraded) {}
				else if(playerRole.whenGuarded == WhenGuarded.killGuarder){ deadPlayersUserId.Add(status.PlayerIdGuradMe); }
			}
		}

		return deadPlayersUserId.ToArray();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
