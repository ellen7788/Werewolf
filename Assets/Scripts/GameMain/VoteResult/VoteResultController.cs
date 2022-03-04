using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class VoteResultController : MonoBehaviour
{
	[SerializeField] GameObject NightCanvas;
	PhotonView photonView;

	// Start is called before the first frame update
	void Start()
	{
		photonView = GetComponent<PhotonView>();
	}

	public void NextButtonClicked()
	{
		photonView.RPC("StartNight", RpcTarget.All);
	}

	[PunRPC]
	void StartNight() {
		// 勝利条件に当てはまったときはリザルト画面に移る
		if(GameInfomation.judgeGameEnd()) {
			string sceneName = "Result";
			PhotonNetwork.LoadLevel(sceneName);
		}

		NightCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
