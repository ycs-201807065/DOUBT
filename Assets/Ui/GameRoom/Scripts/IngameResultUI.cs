using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class IngameResultUI : NetworkBehaviour
{
    public static IngameResultUI InstanceResult;

    [SerializeField]
    public Text endTimerText;  //텍스트용
    [SyncVar(hook = nameof(SetEndTimerText_Hook))]
    private int endTimer;  //실제 시간용
    public void SetEndTimerText_Hook(int _, int value) {
        if (value >= 0) {
            StartCoroutine(EndTrainTimer());
            var players = FindObjectsOfType<MafiaRoomPlayer>();
            endTimerText.text = string.Format("{0}초 후 게임을 재시작 합니다 \n 현재 플레이어 수 : {1}", value, players.Length);
            if(players.Length < 3) {
                endTimerText.text = string.Format("플레이어 인원수가 부족하여 {0}초 후에 나가집니다. \n 현재 플레이어 수 : {1}", (value / 2 + 1), players.Length);
            }
        }
        else {
            reGame();
            StopCoroutine(EndTrainTimer());
        }
    }

    [SerializeField]
    private GameObject outtroImageObj;

    [SerializeField]
    private Text informationText;

    [SerializeField]
    private GameObject trainImageObj;

    [SerializeField]
    private GameObject resultObj;

    [SerializeField]
    private GameObject trainTimeObj;

    [SerializeField]
    private GameObject medicTimeObj;

    [SerializeField]
    private Image gradientImg;

    [SerializeField]
    private ResultCharacter myCharacter;

    [SerializeField]
    private List<ResultCharacter> otherCharacters = new List<ResultCharacter>();

    [SerializeField]
    private List<GameObject> resultImageObj = new List<GameObject>();

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private bool exitTimeConfig = true;    // 방폭파 시간제약

    [SyncVar]
    public bool endGame = false;

    private void Awake() {
        InstanceResult = this;
    }

    public void EndGame() {
        endGame = true;
        StopCoroutine(IngameUIManager.Instance.TrainTimerCoroutine());
        StopCoroutine(IngameUIManager.Instance.GameLifeCheckCoroutine());
        canvasGroup.alpha = 1;
        endTimer = 10;
        SkillUIManager.mInstance.ResetText();
        DeadCameraFind.dcfInstance.ResetText();
        StartCoroutine(ShowTrainSequence());
    }

    //게임 재시작 코루틴
    public IEnumerator EndTrainTimer() {
        yield return new WaitForSeconds(1.0f);  //대기시간
        if (isServer) {
            endTimer--;
            var players = FindObjectsOfType<MafiaRoomPlayer>();
            var manager = NetworkManager.singleton as MafiaRoomManager;
            if(players.Length < 3) {
                if(exitTimeConfig) {
                    endTimer = 5;
                    exitTimeConfig = false;
                }

                if(endTimer <= 0) {
                    manager.StopHost();
                }
            }
        }
        yield return null;
    }
    //게임 재시작 함수
    public void reGame() {
        // Room Manager가 serverChangeScene함수를 이용해서 Gameplay Scene으로 변경한다
        var manager = NetworkManager.singleton as MafiaRoomManager;
        var players = FindObjectsOfType<MafiaRoomPlayer>();
        if(players.Length < 3) {
            if(isServer) {
                ConnectOutMessage.messageCheck = false;
            }
            manager.StopHost();
        } 
        else {
            manager.ServerChangeScene(manager.GameplayScene);
        }
        
    }

    public void OnClickExitButton()
    {
        var manager = NetworkManager.singleton as MafiaRoomManager;
        if(!isServer){
            ConnectOutMessage.messageCheck = false;
            manager.StopClient();
        } else {
            ConnectOutMessage.messageCheck = false;
            manager.StopHost();
        }
    }

    // 게임결과 시퀀스
    public IEnumerator ShowTrainSequence()
    {
        trainTimeObj.SetActive(false);
        medicTimeObj.SetActive(false);
        trainImageObj.SetActive(true);
        ShowTrainArrival();   // 아웃트로
        yield return new WaitForSeconds(4.0f);
        outtroImageObj.SetActive(false);
        trainImageObj.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        ShowGameResult();
        resultObj.SetActive(true);
    }
    // 게임 결과 보여주기
    public void ShowGameResult()
    {
        var players = GameSystem.Instance.GetPlayerList();
        int i = 0;
        string playerType = null;

        foreach (var player in players) {
            if(player.playerType == EPlayerType.Infection) {
                otherCharacters[4].SetResultCharacter(player.nickname, InfectionKillCount.Instance.killNum.ToString());
                otherCharacters[4].gameObject.SetActive(true);
            } else {
                if(player.playerType == EPlayerType.WinResearcher) {
                    playerType = "생존";
                } else if(player.playerType == EPlayerType.Ghost) {
                    playerType = "사망";
                } else {
                    playerType = "연구원";
                }
                otherCharacters[i].SetResultCharacter(player.nickname, playerType);
                otherCharacters[i].gameObject.SetActive(true);
                i++;
            }
        }
    }

    // 아웃트로 이미지 보여주기
    public void ShowTrainArrival()
    {
        bool root = false;
        var players = GameSystem.Instance.GetPlayerList();

        foreach (var player in players) {
            if(player.playerType == EPlayerType.WinResearcher) {
                root = true;
                break;
            }
        }

        if(root) {
            outtroImageObj.SetActive(true);
            informationText.text = "게임 결과를 불러오는 중 입니다";
            return;
        } else {
            outtroImageObj.SetActive(true);
            informationText.text = "게임 결과를 불러오는 중 입니다";
            return;
        }
    }

}
