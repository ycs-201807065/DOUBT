using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {
    public static Mission mInstance;

    public float missionCountTimer;  //현재 미션중인 시간
    public float missionNeedTimer;  //미션 완료에 필요한 시간

    
    public static bool isMissionClear;  //미션 클리어했는지 확인

    void Awake() {
        mInstance = this;
        
    }

    public void missionObject()
    {
        isMissionClear = false;
        missionNeedTimer = GameRuleStore.grInstance.missionTimer;
        Debug.Log("임무 필요 시간 : " + GameRuleStore.grInstance.missionTimer);
        StartCoroutine("OneTimer", 0.1);
    }

    IEnumerator OneTimer(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        var players = GameSystem.Instance.GetPlayerList();
        // 연구원이 미션 아레아에 있다면 자동으로 타이머 증가하기
        foreach (var player in players) {
            if (player.playerType == EPlayerType.Researcher) {
                //누군가 미션 완료
                if (player.missionClearTimer >= missionNeedTimer && player.Mission_Area == true) {
                    //미션 깬거 알려주기
                    isMissionClear = true;
                    //플레이어 미션 진행도 초기화, 총알 지급
                    player.bulletAmmo++;
                    player.missionClearTimer = 0;
                    player.MissionSlider.GetComponent<Slider>().value = 0;
                }
                //미션 완료 아닐때
                else if (player.missionClearTimer < missionNeedTimer && player.Mission_Area == true) {
                    player.missionClearTimer += 0.1f;
                }
            }
        }
        StartCoroutine("OneTimer", 0.1);  //무조건 반복
    }
}
