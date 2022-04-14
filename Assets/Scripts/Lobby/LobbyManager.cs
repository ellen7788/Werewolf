using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
	[SerializeField] InputField playerNameInputField;
	[SerializeField] Button startButton;
	[SerializeField] Text ErrorMessage;

	private const string RoomName = "room";

	// Start is called before the first frame update
	void Start()
	{
		
	}
	
	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
	}

	public void Connect()
	{
		if (chackName()){
			startButton.interactable = false;
			PhotonNetwork.NickName = playerNameInputField.text;
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	private bool chackName()
	{
		if (playerNameInputField == null) {
			return false;
		}
		else if (playerNameInputField.text == "") {
			return false;
		}

		return true;
	}

	public override void OnConnectedToMaster()
	{
		RoomOptions options = new RoomOptions();
		options.PublishUserId = true;
		options.MaxPlayers = 20;
		PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		startButton.interactable = true;
	}

	public override void OnJoinedRoom()
	{
		// string sceneName = "UnityChanRoom";
		string sceneName = "SettingRoom";
		PhotonNetwork.LoadLevel(sceneName);
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		if(returnCode == 32764) ErrorMessage.text = "<color=red>ゲーム中です、終了までしばらくお待ちください。</color>";
		else ErrorMessage.text = "入室に失敗しました";

		PhotonNetwork.Disconnect();

		startButton.interactable = true;
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
