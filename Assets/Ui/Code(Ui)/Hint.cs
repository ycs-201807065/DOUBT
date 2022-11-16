using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    public static Hint Instance;
    public static bool isHint;
    [SerializeField]
    private GameObject hintObject;
    [SerializeField]
    private GameObject researcherHint;
    [SerializeField]
    private GameObject infectionHint;
    
    private void Awake() {
        Instance = this;
    }

    void Update()
    {
        InGamePlayMovement myPlayer = null;
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players) {
            if (player.hasAuthority) {
                myPlayer = player;
                break;
            }
        }
        //감염체라면
        if(myPlayer.playerType == EPlayerType.Infection) {
            if (isHint == false && InGameMenu.hintButton == true) {
                isHint = true;
                hintObject.SetActive(true);
                infectionHint.SetActive(true);
            }
            else if (isHint == true && Input.GetButtonDown("exit")) {
                isHint = false;
                hintObject.SetActive(false);
                infectionHint.SetActive(false);
                InGameMenu.hintButton = false;
            }
        }
        //감염체가 아니라면
        else {
            if (isHint == false && InGameMenu.hintButton == true) {
                isHint = true;
                hintObject.SetActive(true);
                researcherHint.SetActive(true);
            }
            else if (isHint == true && Input.GetButtonDown("exit")) {
                isHint = false;
                hintObject.SetActive(false);
                researcherHint.SetActive(false);
                InGameMenu.hintButton = false;
            }
        }
    }

    public void OpenHint(int type)
    {
        if(type == 1) { // 연구원 도움말 열기
            isHint = true;
            hintObject.SetActive(true);
            researcherHint.SetActive(true);
            InGameMenu.hintButton = true;
        } else if(type == 2) {  // 감염체 도움말 열기
            isHint = true;
            hintObject.SetActive(true);
            infectionHint.SetActive(true);
            researcherHint.SetActive(false);
            InGameMenu.hintButton = true;
        }
    }
}
