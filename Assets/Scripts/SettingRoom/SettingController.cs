using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class SettingController : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject canvas;
	[SerializeField] GameObject participantTotalText;
	[SerializeField] GameObject roleTotalText;
	[SerializeField] GameObject content;
	[SerializeField] GameObject roleNumSetPanel;
	[SerializeField] GameObject playerList;
	[SerializeField] GameObject warningText;
	Text playerListText;
	Roles roles;

	// Start is called before the first frame update
	void Start()
	{
		if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = true;

		string roleData = Resources.Load<TextAsset>("RoleData").ToString();
		roles = JsonUtility.FromJson<Roles>(roleData);

		foreach (Role role in roles.roles) {
			GameObject newRoleNumSetPanel = Instantiate(roleNumSetPanel, Vector3.zero, Quaternion.identity);
			Text roleName = newRoleNumSetPanel.transform.GetChild(0).GetComponent<Text>();
			roleName.text = role.name_jp;

			Text numText = newRoleNumSetPanel.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>();
			int roleNumFromPhoton = SettingPropetiesExtentions.GetRoomRoleNum(role.name_jp);
			numText.text = roleNumFromPhoton < 0 ? role.defaultNum.ToString() : roleNumFromPhoton.ToString();

			newRoleNumSetPanel.transform.SetParent(content.transform);
		}

		playerListText = playerList.GetComponent<Text>();

		updateParticipantTotal();
		updateRoleTotal();
		updatePlayerList();
	}

	void updateParticipantTotal()
	{
		int num = PhotonNetwork.PlayerList.Length;
		participantTotalText.GetComponent<Text>().text = "参加人数：" + num + "人";
	}

	void updateRoleTotal()
	{
		int count = getRoleTotal();

		roleTotalText.GetComponent<Text>().text = "役職合計：" + count + "人";
	}


	int getRoleTotal()
	{
		int count = 0;
		foreach (Transform rolePanel in content.transform) {
			int num = int.Parse(rolePanel.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text);
			count += num;
		}
		return count;
	}

	void updatePlayerList ()
	{
		playerListText.text = "";

		foreach (Player player in PhotonNetwork.PlayerList) {
			if (player.IsMasterClient) playerListText.text += player.NickName + "(マスター)" + "\n";
			else playerListText.text += player.NickName + "\n";
		}
	}

	public override void OnPlayerEnteredRoom (Player newPlayer)
	{
		// Debug.Log("Enter: " + newPlayer);
		updateParticipantTotal();
		updatePlayerList();
	}

	public override void OnPlayerLeftRoom (Player otherPlayer)
	{
		// Debug.Log("Left:" + otherPlayer);
		updateParticipantTotal();
		updatePlayerList();
	}

	public void StartButtonClicked ()
	{
		int roleTotal = getRoleTotal();
		int participantTotal = PhotonNetwork.PlayerList.Length;

		if(roleTotal != participantTotal) {
			warningText.SetActive(true);
			return;
		}

		this.GetComponent<PhotonView>().RPC("LoadGameMainScene", RpcTarget.All);
	}

	[PunRPC]
	public void LoadGameMainScene ()
	{
		GameInfomation.init();
		foreach (Transform roleNumSetPanel in content.transform) {
			Text roleName = roleNumSetPanel.GetChild(0).GetComponent<Text>();
			Text numText = roleNumSetPanel.GetChild(1).transform.GetChild(1).GetComponent<Text>();
			string roleNameJp = roleName.text;
			int roleNum = int.Parse(numText.text);

			GameInfomation.SetRoleSetting(roleNameJp, roleNum);
		}

		if (PhotonNetwork.IsMasterClient) {
			DefinitionRole();
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}

		string sceneName = "GameMain";
        PhotonNetwork.LoadLevel(sceneName);
	}

	void DefinitionRole ()
	{
		List<string> rolesList = new List<string>();

		// extract roles and num
		foreach (Transform roleNumSetPanel in content.transform) {
			Text roleName = roleNumSetPanel.GetChild(0).GetComponent<Text>();
			Text numText = roleNumSetPanel.GetChild(1).transform.GetChild(1).GetComponent<Text>();
			int roleNum = int.Parse(numText.text);

			for (int i = 0; i < roleNum; i++) rolesList.Add(roleName.text);
		}

		// Fisher–Yates shuffle
		for (int i = rolesList.Count - 1; i > 0; i--) {
			int j = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
			string tmp = rolesList[i];
			rolesList[i] = rolesList[j];
			rolesList[j] = tmp;
		}

		Player[] players = PhotonNetwork.PlayerList;
		for (int i = 0; i < players.Length; i++) {
			SettingPropetiesExtentions.SetPlayerRole(players[i].UserId, rolesList[i]);
		}
	}

}
