using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public static bool hintButton;
    public static bool chatting;
    public GameObject menuPanel;
    private bool exitButton;
    private bool menuActive;
    private bool fullScreen;
    private InGamePlayMovement myPlayer;

    private void Start() {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players) {
            if (player.hasAuthority) {
                myPlayer = player;
                break;
            }
        }
    }

    private void Update() {
        if(menuActive == false && Input.GetButtonDown("exit") && hintButton == false && chatting == false) {
            menuPanel.SetActive(true);
            menuActive = true;
        } else if(menuActive && Input.GetButtonDown("exit")) {
            menuPanel.SetActive(false);
            menuActive = false;
        }
    }

    public void OnClickHintButton()
    {
        hintButton = true;
        menuActive = false;
        menuPanel.SetActive(false);
    }

    public void OnClickChangeScreenButton() {
        fullScreen = !fullScreen;
        Screen.SetResolution(1024, 768, fullScreen);
    }

    public void OnClickExitButton()
    {
        IngameResultUI.InstanceResult.OnClickExitButton();
    }
}
