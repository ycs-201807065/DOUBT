using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameIntroUI : MonoBehaviour
{
    [SerializeField]
    private GameObject introImageObj;

    [SerializeField]
    private GameObject infectionmateObj;

    [SerializeField]
    private Text PlayerType;

    [SerializeField]
    private Image gradientImg;

    [SerializeField]
    private IntroCharacter myCharacter;

    [SerializeField]
    private List<IntroCharacter> otherCharacters = new List<IntroCharacter>();

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Text informationText;

    // 인트로 보여주기
    public IEnumerator ShowIntroSequence()
    {
        // 인트로 이미지 출력
        introImageObj.SetActive(true);
        informationText.text = "감염체를 선정하는 중입니다";
        yield return new WaitForSeconds(6f);
        introImageObj.SetActive(false);

        // 플레이어 타입 출력
        ShowPlayerType();
        infectionmateObj.SetActive(true);
    }

    // 플레이어 타입 보여주기(시민, 감염체)
    public void ShowPlayerType()
    {
        var players = GameSystem.Instance.GetPlayerList();

        // 인트로 캐릭터 세팅
        InGamePlayMovement myPlayer = null;
        foreach(var player in players)
        {
            if(player.hasAuthority) {
                myPlayer = player;
                break;
            }
        }

        myCharacter.SetIntroCharacter(myPlayer.nickname);

        
        if(myPlayer.playerType == EPlayerType.Infection) {  // 감염체 UI
            PlayerType.text = "감염체";
            PlayerType.color = gradientImg.color = Color.red;
            myCharacter.ChangeInfectionCharacter();
            
            int i = 0;
            foreach(var player in players)
            {
                if(!player.hasAuthority && player.playerType == EPlayerType.Infection) {
                    otherCharacters[i].SetIntroCharacter(player.nickname);
                    otherCharacters[i].gameObject.SetActive(true);
                    i++;
                }
            }
        } else {    // 연구원 UI
            PlayerType.text = "연구원";
            PlayerType.color = gradientImg.color = new Color(0,255,98);
            
            int i = 0;
            foreach(var player in players)
            {
                if(!player.hasAuthority && player.playerType == EPlayerType.Researcher) {
                    otherCharacters[i].SetIntroCharacter(player.nickname);
                    otherCharacters[i].gameObject.SetActive(true);
                    i++;
                }
            }
        }
    }

    public void Close()
    {
        StartCoroutine(FadeOut());
    }

    // 인트로 UI 투명하게 만들기
    private IEnumerator FadeOut()
    {
        var players = GameSystem.Instance.GetPlayerList();

        // 인트로 캐릭터 세팅
        InGamePlayMovement myPlayer = null;
        foreach (var player in players) {
            if (player.hasAuthority) {
                myPlayer = player;
                break;
            }
        }
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
        }
        if(myPlayer.playerType == EPlayerType.Infection) {
            InfectionSpawnUI.isuInstance.SelectStart();
        } else {
            this.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
