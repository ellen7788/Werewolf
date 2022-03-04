using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChoosePopupController : MonoBehaviour
{
    public string chosenUserId; // ChosenPanelController.csでPopupを作成したときに入れられる
    Role myRole;
	NightAction myRoleNightAction;
	PlayerInfo chosenPlayer;
    Text popupText;
    GameObject cancelButton;
    bool confirmed = false;
    bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        myRole = GameInfomation.playerInfoDict[PhotonNetwork.LocalPlayer.UserId].role;
        myRoleNightAction = myRole.nightAction;
        popupText = this.transform.GetChild(0).GetComponent<Text>();
        cancelButton = this.transform.GetChild(2).gameObject;

        chosenPlayer = GameInfomation.playerInfoDict[chosenUserId];
        popupText.text = GetConfirmPopupText();
        confirmed = false;
        cancelButton.SetActive(true);

        started = true;
    }

    void OnEnable(){
        if(!started) return;

        chosenPlayer = GameInfomation.playerInfoDict[chosenUserId];
        popupText.text = GetConfirmPopupText();
        confirmed = false;
        cancelButton.SetActive(true);
    }

    public void ComfirmButtonClicked(){
        if(!confirmed){
            popupText.text = GetPopupText();
            confirmed = true;
            cancelButton.SetActive(false);
        }
        else {
            // ボタン無効化 & 投票先表示
            Transform content = this.transform.parent.GetChild(2).GetChild(0).GetChild(0);
            foreach (Transform choosePanel in content) {
                choosePanel.GetChild(1).GetComponent<Button>().interactable = false;
                if(choosePanel.GetComponent<UserId>().userId == chosenUserId){
                    Text playerText = choosePanel.GetChild(0).GetComponent<Text>();
                    playerText.text += "\n<color=red>この人を選択しました</color>";
                }
            }

            SettingPropetiesExtentions.SetPlayerChoose(PhotonNetwork.LocalPlayer.UserId, chosenUserId);
            this.gameObject.SetActive(false);
        }
    }

    public void CancelButtonClicked(){
        this.gameObject.SetActive(false);
    }

    string GetConfirmPopupText() {
		string popupText = "";
		string chosenUserNickname = chosenPlayer.nickname;

		if(myRoleNightAction == NightAction.suspectWolf) popupText = "「" + chosenUserNickname + "」を\n選択しますか？";
		else if(myRoleNightAction == NightAction.biteToKill) popupText = "「" + chosenUserNickname + "」を\n噛みますか？";
		else if(myRoleNightAction == NightAction.fotuneTelling) popupText = "「" + chosenUserNickname + "」を\n占いますか？";
		else if(myRoleNightAction == NightAction.seeMediumRes) popupText = "「" + chosenUserNickname + "」を\n選択しますか？";
		else if(myRoleNightAction == NightAction.guardOtherPeople) popupText = "「" + chosenUserNickname + "」を\nを護衛しますか？";
		else popupText = "「" + chosenUserNickname + "」を\n選択しますか？";

		return popupText;
	}

    string GetPopupText() {
		string popupText = "";
		string chosenUserNickname = chosenPlayer.nickname;

		if(myRoleNightAction == NightAction.suspectWolf) popupText = "「" + chosenUserNickname + "」を\n選択しました";
		else if(myRoleNightAction == NightAction.biteToKill) popupText = "「" + chosenUserNickname + "」を\n噛みました";
		else if(myRoleNightAction == NightAction.fotuneTelling) {
			if(chosenPlayer.role.fotuneTellerRes == FotuneTellerRes.citizen) popupText = "「" + chosenUserNickname + "」は\n人狼ではありません";
			else if(chosenPlayer.role.fotuneTellerRes == FotuneTellerRes.wolf) popupText = "「" + chosenUserNickname + "」は\n人狼です";
			else popupText = "「" + chosenUserNickname + "」は\n占い結果が定義されていません";
		}
		else if(myRoleNightAction == NightAction.seeMediumRes) popupText = "「" + chosenUserNickname + "」を\n選択しました";
		else if(myRoleNightAction == NightAction.guardOtherPeople) popupText = "「" + chosenUserNickname + "」を\nを護衛しました";
		else popupText = "「" + chosenUserNickname + "」を\n選択しました";

		return popupText;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
