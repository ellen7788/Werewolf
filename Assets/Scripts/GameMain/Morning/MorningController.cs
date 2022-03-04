using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MorningController : MonoBehaviour
{
	[SerializeField] GameObject DaytimeCanvas;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = GetComponent<PhotonView>();
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

		DaytimeCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}


	// Update is called once per frame
	void Update()
	{
		
	}
}
