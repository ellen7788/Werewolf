using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ResultController : MonoBehaviour
{
    [SerializeField] GameObject winnerText;
    PhotonView photonView;
    Camp winCamp;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        winCamp = GameInfomation.GetWinCamp();
        string winCampJp = "";
        if(winCamp == Camp.citizen) winCampJp = "市民";
        else if(winCamp == Camp.werewolf) winCampJp = "人狼";
        else if(winCamp == Camp.fox) winCampJp = "妖狐";

        winnerText.GetComponent<Text>().text = "「" + winCampJp + "」陣営の\n勝利！";
    }

    public void ToSettingButtonClicked(){
        photonView.RPC("MoveSettingRoom", RpcTarget.All);
    }

    [PunRPC]
    public void MoveSettingRoom(){
        string sceneName = "SettingRoom";
		PhotonNetwork.LoadLevel(sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
