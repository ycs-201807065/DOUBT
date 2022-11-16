using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class IngameUIManager : NetworkBehaviour
{
    public static IngameUIManager Instance;

    [SerializeField]
    private IngameIntroUI ingameIntroUI;
    public IngameIntroUI IngameIntroUI { get { return ingameIntroUI; } }

    [SyncVar(hook = nameof(SetIngameTrainTime_Hook))]
    private int trainTimer;
    [SerializeField]
    private Text trainText;
    public List<GameObject> trainAreaImage = new List<GameObject>();
    private int EndTime;
    [SyncVar]
    public bool escapeActive = false;

    public void SetIngameTrainTime_Hook(int _, int value)
    {
        if (value > 0)
        {
            escapeActive = false;
            StopCoroutine("TrainTimerCoroutine");
            StartCoroutine("TrainTimerCoroutine");
            StartCoroutine("GameLifeCheckCoroutine");
            NoticeUIManager.Instance.NoticeCheck();
            trainText.text = "열차 도착까지 " + trainTimer.ToString() + "초";
        }
        else if (value < -(EndTime))
        {
            StopCoroutine("TrainTimerCoroutine");
            StopCoroutine("GameLifeCheckCoroutine");
            OverTrainTimer();
            NoticeUIManager.Instance.NoticeCheck();
            IngameResultUI.InstanceResult.gameObject.SetActive(true);
            IngameResultUI.InstanceResult.EndGame();

        }
        else
        {
            escapeActive = true;
            TrainArrive();  // 열차도착 시 탈출구 이미지 변경
            StopCoroutine("TrainTimerCoroutine");
            StartCoroutine("TrainTimerCoroutine");
            StartCoroutine("GameLifeCheckCoroutine");
            NoticeUIManager.Instance.NoticeCheck();
            trainText.text = "<color=yellow>열차 출발까지 " + (EndTime + trainTimer).ToString() + "초</color>";
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start() {
        foreach(var trainImage in trainAreaImage){
            trainImage.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public void IngameTrainStart()
    {
        //메인 캔버스 정렬 뒤로 빼줘서 서브 캔버스(카메라 버튼들)보이게 해주기
        //this.GetComponent<Canvas>().sortingOrder = 15000;
        var manager = NetworkManager.singleton as MafiaRoomManager;
        trainTimer = manager.trainTime * 60;  //게임 시간 조정
        EndTime = 30;   // 종료시간 설정
    }

    public int TrainTimeReturn()
    {
        return trainTimer;
    }

    public int EndTimeReturn()
    {
        return EndTime;
    }

    public IEnumerator TrainTimerCoroutine()
    {
        yield return new WaitForSeconds(1.0f);  //대기시간

        if (isServer && IngameResultUI.InstanceResult.endGame == false)
        {
            trainTimer--;     //주석풀면 열차도착 게임종료 활성화
        }

        yield return null;
    }

    private void TrainArrive()
    {
        var players = GameSystem.Instance.GetPlayerList();
        InGamePlayMovement myPlayer = null;

        foreach(var player in players)
        {
            if(player.hasAuthority) {
                myPlayer = player;
                break;
            }
        }

        if (myPlayer.trainSector == 0)
        {
            trainAreaImage[0].SetActive(true);
            trainAreaImage[1].SetActive(true);
            trainAreaImage[0].GetComponent<SpriteRenderer>().enabled = true;
            trainAreaImage[1].GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (myPlayer.trainSector == 1)
        {
            trainAreaImage[2].SetActive(true);
            trainAreaImage[3].SetActive(true);
            trainAreaImage[2].GetComponent<SpriteRenderer>().enabled = true;
            trainAreaImage[3].GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public IEnumerator GameLifeCheckCoroutine()
    {
        if (isServer)
        {
            GameSystem.Instance.RpcGameLifeCheck();
        }
        yield return new WaitForSeconds(1.0f);
    }

    private void OverTrainTimer()
    {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players)
        {
            if (player.playerType == EPlayerType.Researcher)
            {
                Debug.Log("이름 : " + player.nickname);
                player.hp = 0;
                if (isServer)
                {
                    InfectionKillCount.Instance.CmdkillCount();
                }
                player.tag = "DeadPlayer";
                player.playerType = EPlayerType.Ghost;
            }
            else
            {
                continue;
            }
        }
    }

}
