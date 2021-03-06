using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public static class GameInfomation
{
	// ゲームにおいて不変のデータ
	public static List<Role> roleList{ get; }
	public static Dictionary<string, Role> roleDict{ get; }
	public static Dictionary<string, Role> namejpToRoleDict{ get; }

	// ゲーム内で変化するデータ
	public static GameSetting gameSetting{ get; set; }
	public static Dictionary<string, PlayerInfo> playerInfoDict{ get; private set; }
	public static List<RoleSetting> roleSettings{ get; private set; }
	public static int day{ get; set; }
	public static State state{ get; set;}
	public static List<DayActionData> dayActionDataList;

	static GameInfomation() {
		roleList = new List<Role>();
		roleDict = new Dictionary<string, Role>();
		namejpToRoleDict = new Dictionary<string, Role>();

		string roleData = Resources.Load<TextAsset>("RoleData").ToString();
		Roles roles = JsonUtility.FromJson<Roles>(roleData);

		foreach (Role role in roles.roles) {
			roleList.Add(role);
			roleDict.Add(role.role_name, role);
			namejpToRoleDict.Add(role.name_jp, role);
		}
	}

	public static void init(GameSetting gSetting){
		gameSetting = gSetting;
		playerInfoDict = new Dictionary<string, PlayerInfo>();
		roleSettings = new List<RoleSetting>();
		day = 1;
		state = State.night;
		dayActionDataList = new List<DayActionData>();

		dayActionDataList.Add(new DayActionData(day));
	}

	#region PlayerInfoに関する処理
	public static void addPlayerInfo(string UserId, string role_name/* TODO 今はname_jpを入れてるけど, roleにしたい */) {
		PlayerInfo playerInfo = new PlayerInfo(UserId, getNickName(UserId), namejpToRoleDict[role_name]);

		if(playerInfoDict.ContainsKey(UserId)) {
			playerInfoDict.Add(UserId, playerInfo);
		} else {
			playerInfoDict[UserId] = playerInfo;
		}
	}

	public static void SetPlayerIsAlive(string playerId, bool isAlive){
		playerInfoDict[playerId].isAlive = isAlive;
	}

	public static bool GetPlayerIsAlive(string playerId){
		return playerInfoDict[playerId].isAlive;
	}

	public static List<string> GetAlivingPlayersId() {
		List<string> alivePlayers = new List<string>();
		foreach (var player in playerInfoDict) {
			PlayerInfo playerInfo = player.Value;
			if(playerInfo.isAlive) alivePlayers.Add(playerInfo.userId);
		}

		return alivePlayers;
	}
	#endregion

	public static void SetRoleSetting(string roleNameJp, int num){
		Role role = namejpToRoleDict[roleNameJp];
		roleSettings.Add(new RoleSetting(role, num));
	}

	public static void advanceDay(){
		day++;
		dayActionDataList.Add(new DayActionData(day));
	}

	#region dayActionDataに関する更新

	public static void SetChooseInfo(Dictionary<string, string> finChoosePlayer){
		dayActionDataList[day-1].choosingPlayerAndChosenPlayer = new Dictionary<string, string>(finChoosePlayer);
	}

	public static void SetDeadPlayersId(string deadPlayerId){
		SetPlayerIsAlive(deadPlayerId, false);
		dayActionDataList[day-1].deadPlayersId.Add(deadPlayerId);
	}

	public static void SetDeadPlayersId(List<string> deadPlayersId){
		foreach(string id in deadPlayersId){ SetPlayerIsAlive(id, false); }
		dayActionDataList[day-1].deadPlayersId = new List<string>(deadPlayersId);
	}

	public static List<string> GetDeadPlayersId(){
		return dayActionDataList[day-1].deadPlayersId;
	}

	public static void SetVoteInfo(Dictionary<string, string> finVotePlayer){
		dayActionDataList[day-1].votingingPlayerAndVotedPlayer = new Dictionary<string, string>(finVotePlayer);
	}

	public static Dictionary<string, string> GetVoteInfo(){
		return dayActionDataList[day-1].votingingPlayerAndVotedPlayer;
	}

	public static void SetPunishmentedPlayerId(string punishmentedUserId){
		SetPlayerIsAlive(punishmentedUserId, false);
		dayActionDataList[day-1].punishmentedPlayerId = punishmentedUserId;
	}

	public static string GetPunishmentedPlayerId(){
		return dayActionDataList[day-1].punishmentedPlayerId;
	}

	public static void SetDestinyBondedPlayerId(List<string> destinyBondedPlayerId){
		foreach(string id in destinyBondedPlayerId){ SetPlayerIsAlive(id, false); }
		dayActionDataList[day-1].destinyBondedPlayerId = new List<string>(destinyBondedPlayerId);
	}

	public static List<string> GetDestinyBondedPlayerId(){
		return dayActionDataList[day-1].destinyBondedPlayerId;
	}
	#endregion

	public static string getNickName(string userId) {
		string nickname = "";
		foreach (Player player in PhotonNetwork.PlayerList){
			if(player.UserId == userId) {
				nickname = player.NickName;
				break;
			}
		}
		return nickname;
	}

	public static bool judgeGameEnd() {
		int notWolfCount = 0;
		int wolfCount = 0;
		foreach(var playerInfo in playerInfoDict) {
			if (GetPlayerIsAlive(playerInfo.Key)) {
				if(playerInfo.Value.role.isWolf) wolfCount++;
				else notWolfCount++;
			}
		}

		if(wolfCount == 0) return true;
		else if (notWolfCount <= wolfCount) return true;
		return false;
	}

	public static Camp GetWinCamp() {
		int citizenCount = 0;
		int wolfCount = 0;
		int foxCount = 0;

		foreach(var playerInfo in playerInfoDict) {
			if (GetPlayerIsAlive(playerInfo.Key)) {
				if(playerInfo.Value.role.isWolf) wolfCount++;
				else if(playerInfo.Value.role.isFox) foxCount++;
				else citizenCount++;
			}
		}

		if(foxCount > 0) return Camp.fox;
		else if(wolfCount == 0) return Camp.citizen;
		else if(citizenCount <= wolfCount) return Camp.werewolf;
		else return Camp.citizen;
	}
}

public class GameSetting {
	public bool displayVoteInfo;

	public GameSetting(bool displayVoteInfo) {
		this.displayVoteInfo = displayVoteInfo;
	}
}

public class PlayerInfo {
	public string userId;
	public string nickname;
	public Role role;
	public bool isAlive;

	public PlayerInfo(string userId, string nickname, Role role){
		this.userId = userId;
		this.nickname = nickname;
		this.role = role;
		isAlive = true;
	}
}

public enum State {
	night,
	morning,
	daytime,
	vote,
	voteResult
}

public class StatusNight {
	public string PlayerIdBiteMe;
	public string PlayerIdFotuneTellMe;
	public string PlayerIdGuradMe;

	public StatusNight(){
		PlayerIdBiteMe = null;
		PlayerIdFotuneTellMe = null;
		PlayerIdGuradMe = null;
	}
}

public class RoleSetting{
	public Role role;
	public int num;

	public RoleSetting(Role role, int num){
		this.role = role;
		this.num = num;
	}
}

public class DayActionData{
	public int day;
	public Dictionary<string, string> choosingPlayerAndChosenPlayer;
	public List<string> deadPlayersId;
	public Dictionary<string, string> votingingPlayerAndVotedPlayer;
	public string punishmentedPlayerId;
	public List<string> destinyBondedPlayerId;

	public DayActionData(int day){
		this.day = day;
		choosingPlayerAndChosenPlayer = new Dictionary<string, string>();
		deadPlayersId = new List<string>();
		votingingPlayerAndVotedPlayer = new Dictionary<string, string>();
		punishmentedPlayerId = "";
		destinyBondedPlayerId = new List<string>();
	}
}
