using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    /// 사용법
    /// TitleSet("연구원 공지 제목", "감염체 공지 제목");
    /// Noticeset("연구원 공지 내용", "감염체 공지 내용");
    /// Open();
*/

public class NoticeUIManager : MonoBehaviour
{
    public static NoticeUIManager Instance;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text noticeText;
    [SerializeField]
    private CanvasGroup canvasGroup;
    private int timeCheck;

    private void Awake()
    {
        Instance = this;
        DefaultText();
    }

    void Update()
    {
        //NoticeCheck();
    }

    private IEnumerator FadeSet()
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
        }

        yield return new WaitForSeconds(2.0f);  // 공지 표시 시간 조절

        timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
        }

        yield return new WaitForSeconds(3.0f);

        DefaultText();
    }

    public void Open()
    {
        StartCoroutine(FadeSet());
    }

    private void DefaultText()
    {
        Color color;
        ColorUtility.TryParseHtmlString("#00FF62", out color);
        titleText.color = color;
        noticeText.color = color;
        titleText.fontSize = 35;
        noticeText.fontSize = 55;
    }

    public void NoticeCheck()  // 시간, 이벤트 등 발생 확인
    {
        try{
            timeCheck = IngameUIManager.Instance.TrainTimeReturn();
        } catch {
            return;
        }
        
        if (timeCheck == 60)
        {
            TitleSet(null, null);
            NoticeSet("열차도착까지 1분 남았습니다", "열차도착까지 1분 남았습니다");
            Open();
        }
        else if (timeCheck == 0)
        {
            TitleSet("열차 도착", "열차 도착");
            titleText.color = Color.red;
            NoticeSet("열차출발까지 " + IngameUIManager.Instance.EndTimeReturn() + "초 남았습니다", "열차출발까지 " + IngameUIManager.Instance.EndTimeReturn() + "초 남았습니다");
            Open();
        }
    }

    public void TitleSet(string rText, string iText)   // 연구원 공지제목, 감염체 공지제목
    {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players)
        {
            if (player.hasAuthority)
            {
                if (player.playerType == EPlayerType.Infection)
                {
                    titleText.text = iText;
                    break;
                }
                else
                {
                    titleText.text = rText;
                    break;
                }
            }
        }
    }

    public void NoticeSet(string rText, string iText)   // 연구원 공지내용, 감염체 공지내용
    {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players)
        {
            if (player.hasAuthority)
            {
                if (player.playerType == EPlayerType.Infection)
                {
                    noticeText.text = iText;
                    break;
                }
                else
                {
                    noticeText.text = rText;
                    break;
                }
            }
        }
    }
}
