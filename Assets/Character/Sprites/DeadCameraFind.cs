using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadCameraFind : MonoBehaviour {
    public static DeadCameraFind dcfInstance;


    [SerializeField]
    private Text deadInfoText;

    private bool isCheck; //처음 코루틴 체크용


    void Awake() {
        dcfInstance = this;
    }

    void Update() {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players) {
            if (player.hasAuthority && isCheck==false && (player.playerType == EPlayerType.Ghost || player.playerType == EPlayerType.WinResearcher)) {
                deadInfoText.text = "Q키를 눌러 관전 대상을 바꿀 수 있습니다.";
                Invoke("ResetText", 3.0f);
                isCheck = true;
                break;
            }

        }
    }
    public void ResetText() {
        deadInfoText.text = "";
        this.gameObject.SetActive(false);
    }
}