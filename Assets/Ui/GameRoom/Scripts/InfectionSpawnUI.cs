using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfectionSpawnUI : MonoBehaviour
{
    public static InfectionSpawnUI isuInstance;

    [SerializeField]
    private GameObject selectPanel;  // 메인 패널
    [SerializeField]
    private Text selectTimerText;  // 시간
    [SerializeField]
    private List<Transform> spawnTransforms = new List<Transform>();  // 스폰 위치 3곳
    [SerializeField]
    private List<Image> spawnImages = new List<Image>();  // 스폰 위치 이미지 3곳
    [SerializeField]
    private Text selectCurrentText;  // 현재 위치 텍스트
    private int timer;  // 시간
    public int selectSpawn;  // 현재 위치 인덱스


    void Awake() {
        isuInstance = this;
        selectSpawn = 0;
    }

    public void SelectStart() {
        timer = 5;
        selectPanel.SetActive(true);
        selectTimerText.text = timer + "초 후 시작합니다.";
        
        StartCoroutine("SelectTimer");
    }

    void Update() {
        if(timer > 0) {
            var players = GameSystem.Instance.GetPlayerList();
            foreach (var player in players) {
                if (player.playerType == EPlayerType.Infection && player.hasAuthority && Input.GetButtonDown("action")) {
                    player.startGame = false;
                    for(int i = 0; i< spawnImages.Count; i++) {
                        spawnImages[i].enabled = false;
                    }
                    selectSpawn++;
                    if(selectSpawn >= 3) {
                        selectSpawn = 0;
                    }
                    if (selectSpawn == 0) {
                        selectCurrentText.text = "열차 2";
                    }
                    else if (selectSpawn == 1) {
                        selectCurrentText.text = "폐기물";
                    }
                    else if (selectSpawn == 2) {
                        selectCurrentText.text = "창고";
                    }
                    spawnImages[selectSpawn].enabled = true;
                    // 0 열차 2
                    // 1 폐기물
                    // 2 창고
                    break;
                }
            }
        }
    }

    IEnumerator SelectTimer() {
        yield return new WaitForSeconds(1.0f);
        if(timer > 0) {
            timer--;
            selectTimerText.text = timer + "초 후 시작합니다.";
            StartCoroutine("SelectTimer");
        }
        else {
            var players = GameSystem.Instance.GetPlayerList();
            foreach (var player in players) {
                if (player.playerType == EPlayerType.Infection) {
                    player.startGame = true;
                    //player.RpcTeleport(spawnInfectionTransform.position);
                    player.InfectionTeleport(spawnTransforms[selectSpawn].position);
                    Debug.Log("들어왔음 : " + spawnTransforms[selectSpawn].position + " / " + selectSpawn);
                    selectPanel.SetActive(false);
                    Hint.Instance.OpenHint(2);
                    NoticeUIManager.Instance.TitleSet("", "게임 목표");
                    NoticeUIManager.Instance.NoticeSet("", "연구원을 모두 제거해라");
                    NoticeUIManager.Instance.Open();
                    IngameUIManager.Instance.IngameIntroUI.gameObject.SetActive(false);
                    break;
                }
            }
        }
        
    }
}
