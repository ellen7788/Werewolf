using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class ResultController : MonoBehaviour
{
    [SerializeField] GameObject winnerText;
    [SerializeField] GameObject roleList;
    [SerializeField] GameObject DayLogPanels;
    [SerializeField] GameObject DayLogPanelPrefab;
    [SerializeField] GameObject DayLogPanelTitle;
    [SerializeField] GameObject DayLogPanelLog;
    [SerializeField] GameObject aliveList;
    [SerializeField] GameObject deadList;
    PhotonView photonView;
    Camp winCamp;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        SetTexts();
    }

    void SetTexts(){
        // 勝利陣営
        winCamp = GameInfomation.GetWinCamp();
        string winCampJp = "";
        if(winCamp == Camp.citizen) winCampJp = "市民";
        else if(winCamp == Camp.werewolf) winCampJp = "人狼";
        else if(winCamp == Camp.fox) winCampJp = "妖狐";

        winnerText.GetComponent<Text>().text = "「" + winCampJp + "」陣営の勝利！";

        // 役職表示
        Text roleListText = roleList.GetComponent<Text>();
        var firstPlayer = GameInfomation.playerInfoDict.First();
        PlayerInfo firstPlayerInfo = firstPlayer.Value;
        roleListText.text = "「" + firstPlayerInfo.nickname + "」→「" + firstPlayerInfo.role.name_jp + "」";
        foreach(var player in GameInfomation.playerInfoDict.Skip(1)){
            PlayerInfo playerInfo = player.Value;
            string playerNickname = playerInfo.nickname;
            string playerRoleName = playerInfo.role.name_jp;

            roleListText.text += "\n「" + playerNickname + "」→「" + playerRoleName + "」";
        }

        // DayLogPanels
        SetDayLogPanels();

        // 生存者、死亡者をリストに格納
        List<string> alivePlayer = new List<string>();
        List<string> deadPlayer = new List<string>();
        foreach(var player in GameInfomation.playerInfoDict){
            PlayerInfo playerInfo = player.Value;

            if(playerInfo.isAlive) alivePlayer.Add(playerInfo.nickname);
            else deadPlayer.Add(playerInfo.nickname);
        }

        // 生存者
        Text aliveListText = aliveList.GetComponent<Text>();
        aliveListText.text = "";
        for(int i = 0; i < alivePlayer.Count; i++){
            if(i != 0) aliveListText.text += "\n";
            aliveListText.text += "「" + alivePlayer[i] + "」";
        }

        // 死亡者
        Text deadListText = deadList.GetComponent<Text>();
        deadListText.text = "";
        for(int i = 0; i < deadPlayer.Count; i++){
            if(i != 0) deadListText.text += "\n";
            deadListText.text += "「" + deadPlayer[i] + "」";
        }
    }

    void SetDayLogPanels(){
        foreach(DayActionData dayActionData in GameInfomation.dayActionDataList){
            GameObject DayLogPanel = Instantiate(DayLogPanelPrefab, Vector3.zero, Quaternion.identity);
            DayLogPanel.transform.SetParent(DayLogPanels.transform);

            Text DayText = DayLogPanel.transform.GetChild(0).GetComponent<Text>();
            DayText.text = dayActionData.day + "日目";

            string log = "";

            if(dayActionData.day != 1){
                // 朝
                if(dayActionData.deadPlayersId.Count == 0){
                    log = "犠牲者はいませんでした";
                    SetTitleAndLog(DayLogPanel, "朝", log);
                }
                else {
                    log = "以下の人間が死亡しました\n";
                    foreach(string deadPlayerId in dayActionData.deadPlayersId){ log += "「" +  GameInfomation.playerInfoDict[deadPlayerId].nickname + "」"; }
                    SetTitleAndLog(DayLogPanel, "朝", log);
                }

                // 投票
                if(dayActionData.votingingPlayerAndVotedPlayer.Count > 0){
                    log = "";
                    var firstVoteData = dayActionData.votingingPlayerAndVotedPlayer.First();
                    string firstVotePlayerId = firstVoteData.Key;
                    string firstVotedPlayerId = firstVoteData.Value;
                    log += "「" + GameInfomation.playerInfoDict[firstVotePlayerId].nickname + "」が「" + GameInfomation.playerInfoDict[firstVotedPlayerId].nickname + "」に投票";
                    foreach(var voteData in dayActionData.votingingPlayerAndVotedPlayer.Skip(1)){
                        string votePlayerId = voteData.Key;
                        string votedPlayerId = voteData.Value;

                        log += "\n「" + GameInfomation.playerInfoDict[votePlayerId].nickname + "」が「" + GameInfomation.playerInfoDict[votedPlayerId].nickname + "」に投票";
                    }
                    SetTitleAndLog(DayLogPanel, "投票", log);
                } else {
                    break;
                }

                // 投票結果
                log = "";
                string punishmentedPlayerId = dayActionData.punishmentedPlayerId;
                string destinyBondedPlayerId = dayActionData.destinyBondedPlayerId;
                if(punishmentedPlayerId != "") log += "「" + GameInfomation.playerInfoDict[punishmentedPlayerId].nickname + "」が処刑されました";
                if(destinyBondedPlayerId != "") log += "\n「" + GameInfomation.playerInfoDict[destinyBondedPlayerId].nickname + "」が死亡しました";
                SetTitleAndLog(DayLogPanel, "投票結果", log);
            }

            // 夜
            if(dayActionData.choosingPlayerAndChosenPlayer.Count > 0){
                log = "";
                var chooseDataList = dayActionData.choosingPlayerAndChosenPlayer.ToList();
                bool flg = false;
                for(int i = 0; i < chooseDataList.Count; i++){
                    string choosePlayerId = chooseDataList[i].Key;
                    string chosenPlayerId = chooseDataList[i].Value;

                    string choosePlayerNickname = GameInfomation.playerInfoDict[choosePlayerId].nickname;
                    string chosenPlayerNickname = GameInfomation.playerInfoDict[chosenPlayerId].nickname;

                    NightAction choosePlayerNightAction = GameInfomation.playerInfoDict[choosePlayerId].role.nightAction;
                    string message = "「" + GameInfomation.playerInfoDict[choosePlayerId].nickname + "」が「" + GameInfomation.playerInfoDict[chosenPlayerId].nickname + "」を";

                    if(choosePlayerNightAction == NightAction.suspectWolf) /*message += "選択しました"*/ continue;
                    else if(choosePlayerNightAction == NightAction.biteToKill) {
                        if(dayActionData.day == 1) /*message += "選択しました"*/ continue;
                        else message += "噛みました";
                    }
                    else if(choosePlayerNightAction == NightAction.fotuneTelling) message += "占いました";
                    else if(choosePlayerNightAction == NightAction.seeMediumRes) /*message += "選択しました"*/ continue;
                    else if(choosePlayerNightAction == NightAction.guardOtherPeople) message += "護衛しました";
                    else /*message += "選択しました"*/ continue;

                    if(flg) log += "\n" + message;
                    else log += message;

                    flg = true;
                }
                SetTitleAndLog(DayLogPanel, "夜", log);
            }
            else {
                break;
            }
        }
    }

    void SetTitleAndLog(GameObject DayLogPanel, string titleText, string logText){
        GameObject title = Instantiate(DayLogPanelTitle, Vector3.zero, Quaternion.identity);
        title.transform.SetParent(DayLogPanel.transform);
        title.GetComponent<Text>().text = titleText;

        GameObject log = Instantiate(DayLogPanelLog, Vector3.zero, Quaternion.identity);
        log.transform.SetParent(DayLogPanel.transform);
        log.GetComponent<Text>().text = logText;
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
