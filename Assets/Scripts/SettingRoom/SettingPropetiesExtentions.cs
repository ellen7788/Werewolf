using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public static class SettingPropetiesExtentions
{
	private static readonly Hashtable propsToSet = new Hashtable();

	public static readonly string roleToken = "role";
	public static readonly string startTimeStamp = "StartTimeStamp";
	public static readonly string voteToken = "vote";
	public static readonly string chooseToken = "choose";
	public static readonly string leftNotificationToken = "left";

	public static readonly string displayVoteInfoToken = "displayVoteInfo";

	public static void SetPlayerRoleNum (this Player player, string role_jp, int num)
	{
		propsToSet[role_jp] = num;
		player.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static int GetPlayerRoleNum (this Player player, string role_jp) {
		return (player.CustomProperties[role_jp] is int num) ? num : -1;
	}

	public static void SetRoomRoleNum (string role_jp, int num)
	{
		propsToSet[role_jp] = num;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static int GetRoomRoleNum (string role_jp) {
		return (PhotonNetwork.CurrentRoom.CustomProperties[role_jp] is int num) ? num : -1;
	}



	public static void SetGameSettingDisplayVoteInfo (bool flg) {
		propsToSet[displayVoteInfoToken] = flg;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static bool GetGameSettingDisplayVoteInfo () {
		return (PhotonNetwork.CurrentRoom.CustomProperties[displayVoteInfoToken] is bool flg) ? flg : true;
	}



	public static string GetPlayerRoleKey (string userId) {
		return userId + "." + roleToken;
	}

	public static void SetPlayerRole (string userId, string role) {
		propsToSet[GetPlayerRoleKey(userId)] = role;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static string GetPlayerRole (string userId) {
		return (PhotonNetwork.CurrentRoom.CustomProperties[GetPlayerRoleKey(userId)] is string role) ? role : "不明な役職";
	}



	public static string GetStartTimeStampKey() {
		return startTimeStamp;
	}

	public static void SetStartTimeStamp (int timeStamp) {
		propsToSet[GetStartTimeStampKey()] = timeStamp;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static int GetStartTimeStamp () {
		return (PhotonNetwork.CurrentRoom.CustomProperties[GetStartTimeStampKey()] is int timeStamp) ? timeStamp : -1;
	}



	public static string GetPlayerVoteKey (string userId) {
		return userId + "." + voteToken;
	}
	public static void SetPlayerVote (string votingUserId, string votedUserId) {
		propsToSet[GetPlayerVoteKey(votingUserId)] = votedUserId;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static string GetPlayerVote (string userId) {
		return (PhotonNetwork.CurrentRoom.CustomProperties[GetPlayerVoteKey(userId)] is string votedUserId) ? votedUserId : "";
	}


	public static string GetNotifyPlayerLeftKey (string userId) {
		return userId + "." + leftNotificationToken;
	}

	public static void NotifyPlayerLeft(string leftUserId) {
		propsToSet[GetNotifyPlayerLeftKey(leftUserId)] = leftUserId;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}



	public static string GetPlayerChooseKey (string userId) {
		return userId + "." + chooseToken;
	}
	public static void SetPlayerChoose (string choosingUserId, string chosenUserId) {
		propsToSet[GetPlayerChooseKey(choosingUserId)] = chosenUserId;
		PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);
		propsToSet.Clear();
	}

	public static string GetPlayerChoose (string choosingUserId) {
		return (PhotonNetwork.CurrentRoom.CustomProperties[GetPlayerChooseKey(choosingUserId)] is string chosenUserId) ? chosenUserId : "";
	}
}
