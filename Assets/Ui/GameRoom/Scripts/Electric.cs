using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Electric : NetworkBehaviour {
    public UnityEngine.Experimental.Rendering.Universal.Light2D PointLight;
    public static Electric Instance;
    public float electricNeedNumber;

    public int elecCount = 1;

    // Start is called before the first frame update
    void Start() {
        PointLight = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        electricNeedNumber = GameRuleStore.grInstance.missionTimer * 0.6f;
    }

    private void Awake() {
        Instance = this;
    }

    // Update is called once per frame
    void Update() {
        ElectricTouch();
    }

    // 플레이어가 전기실과 상호작용 하는지 확인하는 함수
    public void ElectricTouch() {
        //Debug.Log(GameSystem.Instance.isEle);
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players) {
            if (player.hasAuthority && Input.GetButtonDown("action") && player.Electric_Area && player.isElec == false && player.playerType == EPlayerType.Infection && elecCount>=1) {
                ElectricUIManager.Instance.CmdElecticActive();  // 감염체가 상호작용 시 연구원의 시야 감소
                elecCount--;
                break;
            }

            if (player.hasAuthority && Input.GetButtonDown("action") && player.Electric_Area && player.isElec && player.playerType == EPlayerType.Researcher) {
                Debug.Log("누군가 누름 : " + player.nickname);
                if (player.missionClearTimer == 0) {
                    StartCoroutine(OneTimer(0.1f, player));  //무조건 반복
                }
                //ElectricUIManager.Instance.CmdElecticActive();  // 연구원이 상호작용 시 연구원의 시야 복구
                break;
            }
        }
    }

    IEnumerator OneTimer(float delayTime, InGamePlayMovement player) {
        yield return new WaitForSeconds(delayTime);
        Debug.Log("반복 : " + electricNeedNumber);
        //누군가 미션 완료
        if (player.missionClearTimer >= electricNeedNumber && player.Electric_Area == true) {
            //미션 깬거 알려주기
            ElectricUIManager.Instance.CmdElecticActive();
            //플레이어 미션 진행도 초기화
            player.missionClearTimer = 0;
            player.MissionSlider.GetComponent<Slider>().value = 0;
        }
        //미션 완료 아닐때
        else if (player.missionClearTimer < electricNeedNumber && player.Electric_Area == true) {
            player.missionClearTimer += 0.1f;
            StartCoroutine(OneTimer(0.1f, player));  //무조건 반복
        }


    }
}
