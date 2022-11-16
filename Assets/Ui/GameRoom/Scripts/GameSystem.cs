using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;

public class GameSystem : NetworkBehaviour
{
    public static GameSystem Instance;
    private List<InGamePlayMovement> players = new List<InGamePlayMovement>();
    public List<GameObject> trainAreas = new List<GameObject>();
    [SerializeField]
    private GameObject MessageBox;
    [SerializeField]
    public List<GameObject> missionsTransform;//미션 오브젝트들 미션 오브젝트들 private->public으로 수정함(미니맵에 미션위치 표현때문에)
    [SerializeField]
    private Transform spawnTransform;

    [SerializeField]
    private float spawnDistance;
    private bool endGame;   // 게임종료 여부

    public void AddPlayer(InGamePlayMovement player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    private IEnumerator GameReady()
    {
        DefaultGameStart();
        
        var manager = NetworkManager.singleton as MafiaRoomManager;
        while (manager.roomSlots.Count != players.Count)
        {
            yield return null;
        }

        // 감염체 선정
        for (int i = 0; i < 1; i++)
        {
            var player = players[Random.Range(0, players.Count)];
            if (player.playerType != EPlayerType.Infection)
            {
                player.playerType = EPlayerType.Infection;
            }
            else
            {
                i--;
            }
        }

        // 열차도착지점 랜덤지정
        for (int i = 0; i < 1; i++)
        {
            int random = Random.Range(0, trainAreas.Count);
            var trainArea = trainAreas[random];
            if (trainArea.activeSelf == false)
            {
                TrainSectorSetting(random);
                trainArea.tag = "Train_Area";
                trainArea.SetActive(true);
                trainArea.AddComponent<Rigidbody2D>();
                trainArea.AddComponent<TilemapCollider2D>();
                trainArea.AddComponent<CompositeCollider2D>();
                Rigidbody2D rig = trainArea.GetComponent<Rigidbody2D>();
                rig.bodyType = RigidbodyType2D.Static;
                TilemapCollider2D TC = trainArea.GetComponent<TilemapCollider2D>();
                TC.usedByComposite = true;
                CompositeCollider2D CC = trainArea.GetComponent<CompositeCollider2D>();
                CC.isTrigger = true;
            }
            Debug.Log("탈출지점 : " + (random + 1));
        }

        int missionNumber = Random.Range(0, 3);
        MissionSectorSetting(missionNumber);

        for (int i = 0; i < players.Count; i++)
        {
            float radian = (2f * Mathf.PI) / players.Count;
            radian *= i;
            //연구원, 감염체 스폰 위치 지정
            if (players[i].playerType == EPlayerType.Researcher)
            {
                players[i].RpcTeleport(spawnTransform.position + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));
            }
        }

        yield return new WaitForSeconds(2f);
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(IngameUIManager.Instance.IngameIntroUI.ShowIntroSequence());    // 인트로 보여주기

        yield return new WaitForSeconds(3f);
        IngameUIManager.Instance.IngameIntroUI.Close(); // 인트로 닫고 게임화면 보여주기
        Infection_Change(); // 감염체 캐릭터 변경
        GameStart();    // 플레이어 게임시작 상태로 변경
        SkillUIManager.mInstance.SkillTimerStart();
        IngameUIManager.Instance.IngameTrainStart();
        GameGoalSet();
        NoticeUIManager.Instance.Open();    // 공지 표시
        //DeadCameraFind.dcfInstance.ViewStart();  죽은 플레이어 UI
        Hint_Open();
        SetMissionBox();
        Mission.mInstance.missionObject();

        IngameResultUI.InstanceResult.gameObject.SetActive(false);
    }

    public void SetMissionBox() {
        int missionNumber =0;
        foreach (var player in players)
        {
            missionNumber = player.trainSector;
            break;
        }
        if(GameRuleStore.grInstance.missionArea == 1) {
            if(missionNumber == 0) {
                missionsTransform[0].SetActive(true);
            }
            else if (missionNumber == 1) {
                missionsTransform[3].SetActive(true);
            }
            else if (missionNumber == 2) {
                missionsTransform[4].SetActive(true);
            }
        }
        else if (GameRuleStore.grInstance.missionArea == 2) {
            if (missionNumber == 0) {
                missionsTransform[1].SetActive(true);
                missionsTransform[2].SetActive(true);
            }
            else if (missionNumber == 1) {
                missionsTransform[4].SetActive(true);
                missionsTransform[5].SetActive(true);
            }
            else if (missionNumber == 2) {
                missionsTransform[3].SetActive(true);
                missionsTransform[4].SetActive(true);
            }
        }
        else if (GameRuleStore.grInstance.missionArea == 3) {
            if (missionNumber == 0) {
                missionsTransform[1].SetActive(true);
                missionsTransform[3].SetActive(true);
                missionsTransform[5].SetActive(true);
            }
            else if (missionNumber == 1) {
                missionsTransform[0].SetActive(true);
                missionsTransform[3].SetActive(true);
                missionsTransform[5].SetActive(true);
            }
            else if (missionNumber == 2) {
                missionsTransform[2].SetActive(true);
                missionsTransform[4].SetActive(true);
                missionsTransform[1].SetActive(true);
            }
        }
        else if (GameRuleStore.grInstance.missionArea == 4) {
            if (missionNumber == 0) {
                missionsTransform[0].SetActive(true);
                missionsTransform[2].SetActive(true);
                missionsTransform[3].SetActive(true);
                missionsTransform[4].SetActive(true);
            }
            else if (missionNumber == 1) {
                missionsTransform[1].SetActive(true);
                missionsTransform[2].SetActive(true);
                missionsTransform[3].SetActive(true);
                missionsTransform[5].SetActive(true);
            }
            else if (missionNumber == 2) {
                missionsTransform[0].SetActive(true);
                missionsTransform[1].SetActive(true);
                missionsTransform[4].SetActive(true);
                missionsTransform[5].SetActive(true);
            }
        }
    }
    public List<InGamePlayMovement> GetPlayerList()
    {
        return players;
    }

    private void TrainSectorSetting(int random){
        foreach (var player in players)
        {
            player.trainSector = random;
        }
    }
    private void MissionSectorSetting(int randomNumber){
        foreach (var player in players)
        {
            player.missionSector = randomNumber;
        }
    }

    private void GameStart()
    {
        foreach (var player in players)
        {
            if(player.playerType == EPlayerType.Researcher)
            {
                player.startGame = true;
            }
        }
    }

    private void DefaultGameStart()
    {
        foreach (var player in players)
        {
            player.startGame = false;
        }
    }

    private void Infection_Change()
    {
        foreach (var player in players)
        {
            if (player.playerType == EPlayerType.Infection)
            {
                player.anim.SetBool("isInfection", true);
                player.anim.SetBool("isChange", true);
                player.anim.SetInteger("vAxisRaw", -1);
                break;
            }
        }
    }

    private void Hint_Open()
    {
        foreach (var player in players)
        {
            if (player.playerType == EPlayerType.Researcher)
            {
                Hint.Instance.OpenHint(1);
            }
        }
    }

    private void GameGoalSet()  // 게임 목표 설정
    {
        NoticeUIManager.Instance.TitleSet("게임 목표", "");
        NoticeUIManager.Instance.NoticeSet("열차를 타고 탈출해라", "");
    }

    [ClientRpc]
    public void RpcGameLifeCheck()    // 게임 종료 조건 확인
    {
        bool researcherExist = false;
        bool infectionExist = false;

        foreach (var player in players)
        {
            if (player.playerType == EPlayerType.Researcher)
            {
                researcherExist = true;
            }
            else if (player.playerType == EPlayerType.Infection)
            {
                infectionExist = true;
            }
            else
            {
                continue;
            }
        }

        Debug.Log("researcherExist : " + researcherExist);
        Debug.Log("infectionExist : " + infectionExist);

        if ((researcherExist == false || infectionExist == false) && endGame == false)
        {
            endGame = true;
            IngameResultUI.InstanceResult.gameObject.SetActive(true);
            IngameResultUI.InstanceResult.EndGame();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(var train in trainAreas){
            train.SetActive(false);
        }
        if (isServer)
        {
            StartCoroutine(GameReady());
        }
    }

}
