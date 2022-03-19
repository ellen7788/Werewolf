using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameMainController : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject RoleText;
	[SerializeField] Text RoleListText;

	string myRole;

	// Start is called before the first frame update
	void Start()
	{
		string roleListText = "";
		foreach(RoleSetting roleSetting in GameInfomation.roleSettings){
			if(roleSetting.num > 0) roleListText += roleSetting.role.name_jp + " " + roleSetting.num + "\n";
		}
		RoleListText.text = roleListText;
	}

	public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
		string key = SettingPropetiesExtentions.GetPlayerRoleKey(PhotonNetwork.LocalPlayer.UserId);
		if (changedProps.ContainsKey(key)) {
			RoleText.GetComponent<Text>().text = "あなたの役職は\n「" + changedProps[key] + "」です。";
			myRole = changedProps[key].ToString();
			RoleText.SetActive(true);
		}

		foreach (System.Collections.DictionaryEntry de in changedProps) {
			if(de.Key.ToString().Split('.').Length >= 2 && de.Key.ToString().Split('.')[1] == SettingPropetiesExtentions.roleToken) {
				string userId = de.Key.ToString().Split('.')[0];
				string role_jp = de.Value.ToString();

				GameInfomation.addPlayerInfo(userId, role_jp);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
