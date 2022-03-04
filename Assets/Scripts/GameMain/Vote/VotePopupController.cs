using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class VotePopupController : MonoBehaviour
{
    public string votedUserId; // ChosenPanelController.csでPopupを作成したときに入れられる

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ComfirmButtonClicked(){
        // ボタン無効化
		Transform content = this.transform.parent.GetChild(2).GetChild(0).GetChild(0);
		foreach (Transform choosePanel in content) {
			choosePanel.GetChild(1).GetComponent<Button>().interactable = false;
            if(choosePanel.GetComponent<UserId>().userId == votedUserId){
                Text playerText = choosePanel.GetChild(0).GetComponent<Text>();
                playerText.text += "\n<color=red>この人に投票しました</color>";
            }
		}

        SettingPropetiesExtentions.SetPlayerVote(PhotonNetwork.LocalPlayer.UserId, votedUserId);
        this.gameObject.SetActive(false);
    }

    public void CancelButtonClicked(){
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
