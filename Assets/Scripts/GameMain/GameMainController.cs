using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameMainController : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject RoleText;

	string myRole;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
		string key = SettingPropetiesExtentions.GetPlayerRoleKey(PhotonNetwork.LocalPlayer.UserId);
		if (changedProps.ContainsKey(key)) {
			RoleText.GetComponent<Text>().text = "あなたの役職は\n「" + changedProps[key] + "」です。";
			myRole = changedProps[key].ToString();
			RoleText.SetActive(true);
		}

		foreach (System.Collections.DictionaryEntry de in changedProps) {
			if(de.Key.ToString().Split('.')[1] != SettingPropetiesExtentions.roleToken) continue;

			string userId = de.Key.ToString().Split('.')[0];
			string role_jp = de.Value.ToString();

			GameInfomation.addPlayerInfo(userId, role_jp);
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
