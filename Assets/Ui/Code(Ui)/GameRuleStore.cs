using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Mirror;

public struct GameRuleData {
    public float playerSpeed;
    public int missionTimer;
    public int missionArea;
}

public class GameRuleStore : NetworkBehaviour {
    public static GameRuleStore grInstance;

    [SyncVar(hook = nameof(SetSpeedText_Hook))]
    public float playerSpeed;
    [SerializeField]
    private Text playerSpeedText;
    public void SetSpeedText_Hook(float _, float value) {
        playerSpeedText.text = string.Format("{0}", value);
        UpdateGameRuleOverview();
    }
    public void OnSpeedChange(bool isPlus) {
        if (isServer) {
            playerSpeed = Mathf.Clamp(playerSpeed + (isPlus ? 0.5f : -0.5f), 0.5f, 3.5f);
        }
    }

    [SyncVar(hook = nameof(SetMissionTimerText_Hook))]
    public int missionTimer;
    [SerializeField]
    private Text missionTimerText;
    public void SetMissionTimerText_Hook(int _, int value) {
        missionTimerText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnMissionTimerChange(bool isPlus) {
        if (isServer) {
            missionTimer = Mathf.Clamp(missionTimer + (isPlus ? 5 : -5), 10, 30);
        }
    }

    [SyncVar(hook = nameof(SetMissionAreaText_Hook))]
    public int missionArea;
    [SerializeField]
    private Text missionAreaText;
    public void SetMissionAreaText_Hook(int _, int value) {
        missionAreaText.text = string.Format("{0}", value);
        UpdateGameRuleOverview();
    }
    public void OnMissionAreaChange(bool isPlus) {
        if (isServer) {
            missionArea = Mathf.Clamp(missionArea + (isPlus ? 1 : -1), 1, 4);
        }
    }
    
    //영상 11장 0:38
    [SyncVar(hook = nameof(SetRolePlayerCount_Hook))]
    private int rolePlayerCount; // 영상 11장
    public void SetRolePlayerCount_Hook(int _, int value)
    {
        UpdateGameRuleOverview();
    }
    [SyncVar(hook = nameof(SetRoleTrainTime_Hook))]
    private int roleTrainTime; // 영상 11장
    public void SetRoleTrainTime_Hook(int _, int value) {
        UpdateGameRuleOverview();
    }

    [SerializeField]
    private Text gameRuleOverview;

    public void UpdateGameRuleOverview() {
        var manager = NetworkManager.singleton as MafiaRoomManager;
        StringBuilder sb = new StringBuilder();
        sb.Append($"열차 도착 시간 : {roleTrainTime}\n");
        sb.Append($"최대 플레이어 수 : {rolePlayerCount}\n");  // 영상 11장 manager.playerCount -> playerCount
        sb.Append($"플레이어 이동속도 : {playerSpeed}\n");
        sb.Append($"임무 완료 시간 : {missionTimer}\n");
        sb.Append($"임무 수행 위치 : {missionArea}\n");
        gameRuleOverview.text = sb.ToString();
    }

    private void SetDefaultGameRule() {
        playerSpeed = 1.5f;
        missionTimer = 10;
        missionArea = 2;
    }

    void Awake() {
        grInstance = this;
    }
    // Start is called before the first frame update
    void Start() {
        SetDefaultGameRule();
        UpdateGameRuleOverview();
        
        if (isServer) // 영상 11장 0:43
        {
            var manager = NetworkManager.singleton as MafiaRoomManager; // 영상 11장 0:51
            rolePlayerCount = manager.playerCount; //영상 11장 0:51
            roleTrainTime = manager.trainTime; //영상 11장 0:51
            SetDefaultGameRule(); //SetRecommendGameRule();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
