using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SkillUIManager : NetworkBehaviour {
    public static SkillUIManager mInstance;

    [SerializeField]
    public Text skillTimerText;  //텍스트용

    [SerializeField]
    private Text skillInfoNameText;  //스킬 이름
    [SerializeField]
    private Text skillInfoText;  //스킬 설명

    [SyncVar(hook = nameof(SetMedicTimerText_Hook))]
    private int skillTimer;  //실제 시간용
    public void SetMedicTimerText_Hook(int _, int value) {
        if (value > 0) {
            StartCoroutine("SkillTimerCoroutine");
            skillTimerText.text = string.Format("감염체 변이 강화까지 : {0}초", value);
        }
        else {
            skillTimerText.text = "감염체 변이가 완료되었습니다.";
            StartCoroutine("GetSkillTextCoroutine");
            StopCoroutine("SkillTimerCoroutine");
        }
    }

    private void SkillSetting(int random) {
        Debug.Log("====첫 스킬 확인====");
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var skillAddPlayer in players) {
            skillAddPlayer.randomSkill = random;
            Debug.Log(skillAddPlayer.nickname + " : " + skillAddPlayer.randomSkill);
        }
    }

    public void ResetText() {
        //초기값으로 설정
        skillInfoNameText.text = "";
        skillInfoText.text = "";
        this.gameObject.SetActive(false);
        skillInfoText.gameObject.SetActive(false);
        skillTimer = 60;
        StopCoroutine("SkillTimerCoroutine");
    }

    private void Awake() {
        mInstance = this;
    }
    public void SkillTimerStart() {
        skillTimer = 60;
        if (isServer) {
            int skillNumber = Random.Range(1, 4);  //1~3값 랜덤으로 준다.
            SkillSetting(skillNumber);
        }
        StartCoroutine("SkillSelectCoroutine", skillTimer);

    }

    private IEnumerator SkillSelectCoroutine(float delay) {
        yield return new WaitForSeconds(delay);  //대기시간
        var players = GameSystem.Instance.GetPlayerList();
        Debug.Log("====두번째 스킬 확인====");
        foreach (var player in players) {
            Debug.Log(player.nickname + " : " + player.randomSkill + " : " + player.playerType);
            if (player.hasAuthority && player.playerType == EPlayerType.Infection) {
                player.GetSkill();  //스킬 획득
                if (player.randomSkill == 1) {
                    skillInfoNameText.text = "점막 변이(G)";
                    skillInfoText.text = "점막을 설치합니다.\n주변 연구원은 느려지고 자신은 빨라집니다.\n중첩 불가능, 4회 사용";
                }
                else if (player.randomSkill == 2) {
                    skillInfoNameText.text = "신체 변형(G)";
                    skillInfoText.text = "연구원 모습으로 변형합니다.\n공격후 이동하면 변신이 풀립니다.\n3회 사용";
                }
                else if (player.randomSkill == 3) {
                    skillInfoNameText.text = "땅굴 변형(G)";
                    skillInfoText.text = "땅굴에서 K키를 누르면 다른 땅굴로 이동합니다.\n2개의 땅굴이 설치되어 있어야 합니다.\n2회 사용, 재사용 3초";
                }
                break;
            }
        }
    }


    private IEnumerator SkillTimerCoroutine() {
        yield return new WaitForSeconds(1.0f);  //대기시간
        if (isServer) {
            skillTimer--;
        }
        yield return null;
    }

    private IEnumerator GetSkillTextCoroutine() {

        yield return new WaitForSeconds(3f);  //대기시간
        skillTimerText.text = "";
    }
}