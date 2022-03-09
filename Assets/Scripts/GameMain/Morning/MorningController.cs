using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MorningController : MonoBehaviour
{
	[SerializeField] GameObject DaytimeCanvas;
	[SerializeField] GameObject DayText;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = GetComponent<PhotonView>();
	}

	void OnEnable(){
		DayText.GetComponent<Text>().text = GameInfomation.day + "日目";
	}

	public void NextButtonClicked()
	{
		photonView.RPC("StartDaytime", RpcTarget.All);
	}

	[PunRPC]
	void StartDaytime() {
		// 勝利条件に当てはまったときはリザルト画面に移る
		if(GameInfomation.judgeGameEnd()) {
			string sceneName = "Result";
			PhotonNetwork.LoadLevel(sceneName);
		}
		else {
			DaytimeCanvas.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}


	// Update is called once per frame
	void Update()
	{
		
	}
}
