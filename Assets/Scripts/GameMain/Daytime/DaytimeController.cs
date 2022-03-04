using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DaytimeController : MonoBehaviour
{
	[SerializeField] GameObject timer;
	[SerializeField] GameObject VoteCanvas;
	PhotonView photonView;
	Text timerText;
	int discussionTime;
	int startTime;
	int elapsedTime;
	int leftTime;

	// Start is called before the first frame update
	void Start()
	{
		photonView = this.GetComponent<PhotonView>();
		timerText = timer.GetComponent<Text>();
	}

	void OnEnable()
	{
		discussionTime = 300 * 1000;
		startTime = PhotonNetwork.ServerTimestamp;
	}

	// Update is called once per frame
	void Update()
	{
		elapsedTime = PhotonNetwork.ServerTimestamp - startTime;
		leftTime = discussionTime - elapsedTime;

		timerText.text = MinSecString(leftTime);

		if(leftTime <= 0) {
			if(PhotonNetwork.IsMasterClient) photonView.RPC("StartVote", RpcTarget.All);
		}
	}

	string MinSecString (int milisec)
	{
		int sec = milisec / 1000 + 1;
		int min = sec / 60;
		milisec %= 1000;
		sec %= 60;
		return min.ToString("00") + ":" + sec.ToString("00")/* + "." + milisec.ToString("000")*/;
	}

	public void SkipButtonClicked(){
		photonView.RPC("StartVote", RpcTarget.All);
	}

	[PunRPC]
	void StartVote ()
	{
		VoteCanvas.SetActive(true);
		this.gameObject.SetActive(false);
	}
}
