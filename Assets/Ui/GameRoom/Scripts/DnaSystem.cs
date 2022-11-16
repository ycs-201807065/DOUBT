using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class DnaSystem : NetworkBehaviour
{
    public static DnaSystem Instance;
    /*
    private int dnaTimerSet;
    [SerializeField]
    private bool dnaReady;
    [SerializeField]
    private bool textCheck;
    [SerializeField]
    private bool playerNumCheck;
    [SerializeField]
    private string[] dnaCheckPlayer = {""};
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Text noticeText1;  // 공지용, 결과확인용
    [SerializeField]
    private Text noticeText2;   // 결과확인용

    [SyncVar(hook = nameof(SetDnaTimerText_Hook))]
    private int dnaTimer;  // 계산용
    public void SetDnaTimerText_Hook(int _, int value) {
        if (value > 0) {
            StartCoroutine(DnaTimerCoroutine());
        }
        else {
            dnaReady = true;
            StopCoroutine(DnaTimerCoroutine());
            StartCoroutine(NoticeCheckCoroutine());
            StartCoroutine(PlayerCheckCoroutine());
        }
    }

    public void DnaTimerStart()
    {
        dnaTimer = 5;
        dnaReady = false;
        textCheck = true;
        playerNumCheck = true;
    }

    private IEnumerator DnaTimerCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        if (isServer) {
            dnaTimer--;
        }
        yield return null;
    }

    private IEnumerator NoticeCheckCoroutine()
    {
        if(dnaReady == true) {
            if(noticeText1.gameObject.activeSelf == true && textCheck == true) {
                yield return new WaitForSeconds(1.0f);
                yield return null;
            } else {
                textCheck = false;
                noticeText1.gameObject.SetActive(true);
                Open();
                StopCoroutine(FadeIn());
                noticeText1.text = "DNA실 이용 가능";
                yield return new WaitForSeconds(5.0f);
                Close();
                StopCoroutine(FadeOut());
                textCheck = true;
                yield return null;
            }
        }
        yield return null;
    }

    public void UseDnaRoom()
    {
        if (dnaReady == true) {
            dnaTimer = 5;
            dnaReady = false;
            playerNumCheck = true;
            StopCoroutine(NoticeCheckCoroutine());
            StopCoroutine(PlayerCheckCoroutine());
            return;
        }
    }

    private void Close()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
        }
    }

    private void Open()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
        }
    }

    private IEnumerator PlayerCheckCoroutine()
    {
        var players = GameSystem.Instance.GetPlayerList();
        InGamePlayMovement myPlayer = null;
        int count = 0;

        if(playerNumCheck == true){
            foreach(var player in players)
            {
                myPlayer = player;
                if(myPlayer.DNA_Area == true && myPlayer.playerType != EPlayerType.Ghost) {
                    count++;
                }
            }
        }

        if(count == 2) {
            playerNumCheck = false;
            DnaDecision();
            UseDnaRoom();
            ResultDnaCheck();
            yield return new WaitForSeconds(5.0f);
            Close();
            StopCoroutine(FadeOut());
            noticeText1.gameObject.SetActive(false);
            noticeText2.gameObject.SetActive(false);
        } else if(count >= 3) {
            noticeText1.gameObject.SetActive(true);
            Open();
            StopCoroutine(FadeIn());
            noticeText1.text = "DNA 체크 구역에 3명 이상이 있습니다.";
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return new WaitForSeconds(1.0f);
        }
        yield return null;
    }

    private void ResultDnaCheck()
    {
        Close();
        StopCoroutine(FadeOut());
        noticeText1.gameObject.SetActive(true);
        noticeText2.gameObject.SetActive(true);
        Open();
        StopCoroutine(FadeIn());
        noticeText1.text = string.Format("닉네임 : {0}  ||  {1}", dnaCheckPlayer[0], dnaCheckPlayer[1]);
        Debug.Log("         noticeText1 : " + dnaCheckPlayer[0] + "     " + dnaCheckPlayer[1]);
        noticeText2.text = string.Format("닉네임 : {0}  ||  {1}", dnaCheckPlayer[2], dnaCheckPlayer[3]);
        Debug.Log("         noticeText2 : " + dnaCheckPlayer[2] + "     " + dnaCheckPlayer[3]);
    }

    public void DnaDecision()    // 결과 판정
    {
        var players = GameSystem.Instance.GetPlayerList();
        InGamePlayMovement myPlayer = null;
        int count = 0;

        foreach(var player in players)
        {
            myPlayer = player;
            if(myPlayer.DNA_Area == true && myPlayer.playerType != EPlayerType.Ghost) {
                if (myPlayer.playerType == EPlayerType.Infection) {
                    dnaCheckPlayer[count] = myPlayer.nickname;
                    dnaCheckPlayer[count + 1] = "감염체";
                    count += 2;
                } else {
                    dnaCheckPlayer[count] = myPlayer.nickname;
                    dnaCheckPlayer[count + 1] = "연구원";
                    count += 2;
                }
            }
        }
    } */

    private void Awake() {
        Instance = this;
    }
}
