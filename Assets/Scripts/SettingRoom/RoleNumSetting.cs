using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoleNumSetting : MonoBehaviourPunCallbacks
{
	private Text numText;
	private GameObject roleTotalText;
	private GameObject content;
	private PhotonView pView;
	private string roleName_jp;
	private const int maxNum = 20;

	// Start is called before the first frame update
	void Start()
	{
		numText = this.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>();
		roleTotalText = GameObject.Find("RoleTotalText");
		content = GameObject.Find("Content");
		pView = GetComponent<PhotonView>();
		roleName_jp = this.transform.GetChild(0).GetComponent<Text>().text;
	}

	public void IncrementNum()
	{
		if(numText.text != maxNum.ToString()){
			int nextNum = int.Parse(numText.text) + 1;
			SettingPropetiesExtentions.SetRoomRoleNum(roleName_jp, nextNum);
			// numText.text = nextNum.ToString();
			// updateRoleTotal();
		}
	}

	public void DecrementNum()
	{
		if(numText.text != "0"){
			int nextNum = int.Parse(numText.text) - 1;
			SettingPropetiesExtentions.SetRoomRoleNum(roleName_jp, nextNum);
			// numText.text = nextNum.ToString();
			// updateRoleTotal();
		}
	}

	void updateRoleTotal()
	{
		int count = 0;
		foreach (Transform rolePanel in content.transform) {
			int num = int.Parse(rolePanel.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text);
			count += num;
		}

		roleTotalText.GetComponent<Text>().text = "役職合計：" + count + "人";
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
		if (changedProps.ContainsKey(roleName_jp)) {
			numText.text = ((int)changedProps[roleName_jp]).ToString();
			updateRoleTotal();
		}
	}

	public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
		if (changedProps.ContainsKey(roleName_jp)) {
			numText.text = ((int)changedProps[roleName_jp]).ToString();
			updateRoleTotal();
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
