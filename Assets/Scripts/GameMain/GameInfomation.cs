using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public static class GameInfomation
{
	public static Dictionary<string, PlayerInfo> playerInfoDict{ get; private set; }

	public static List<Role> roleList{ get; }
	public static Dictionary<string, Role> roleDict{ get; }
	public static Dictionary<string, Role> namejpToRoleDict{ get; }

	public static int day{ get; set; }
	public static State state{ get; set;}
	public static List<string> punishmentedPlayersId{ get; private set; }

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

	public static void init(){
		playerInfoDict = new Dictionary<string, PlayerInfo>();
		day = 1;
		state = State.night;
		punishmentedPlayersId = new List<string>();
	}

	public static void addPlayerInfo(string UserId, string role_name/* TODO 今はname_jpを入れてるけど, roleにしたい */) {
		PlayerInfo playerInfo = new PlayerInfo(UserId, getNickName(UserId), namejpToRoleDict[role_name]);

		if(playerInfoDict.ContainsKey(UserId)) {
			playerInfoDict.Add(UserId, playerInfo);
		} else {
			playerInfoDict[UserId] = playerInfo;
		}
	}

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

	public static void addPunishmentedPlayer(string UserId) {
		punishmentedPlayersId.Add(UserId);
	}

	public static bool judgeGameEnd() {
		int notWolfCount = 0;
		int wolfCount = 0;
		foreach(var playerInfo in playerInfoDict) {
			if (SettingPropetiesExtentions.GetPlayerIsAlive(playerInfo.Key)) {
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
			if (SettingPropetiesExtentions.GetPlayerIsAlive(playerInfo.Key)) {
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

public class PlayerInfo {
	public string userId;
	public string nickname;
	public Role role;

	public PlayerInfo(string userId, string nickname, Role role){
		this.userId = userId;
		this.nickname = nickname;
		this.role = role;
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
