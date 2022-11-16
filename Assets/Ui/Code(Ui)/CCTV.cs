using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CCTV : MonoBehaviour
{
    public static CCTV Instance;
    private Canvas CCTVUI;          // CCTV 화면을 껐다,켰다 해주는 변수
    private GameObject Bright;      // CCTV 빛을 껐다, 켰다 해주는 변수

    public static bool isCCTV;  // CCTV 확인용

    //private bool Ativo = false;     // 플레이어와 cctv의 충돌상태를 구분해주는 변수
    //private UnityEngine.Experimental.Rendering.Universal.Light2D Bright;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isCCTV = false;
        CCTVUI = GetComponentInChildren<Canvas>();
        Bright = GameObject.Find("CCTVLight");  // Game Object를 껐다 켰다 사용할려면 Game Object 초기 상태(inspector)가 활성화(체크 박스 칸)로 해두어야 된다. 
        Bright.SetActive(false);    // Game Object는 SetActive 명령어로 껐다 켰다 사용가능, 컴포넌트는 enabled로 사용가능.
    }  // ◆ 스타트 괄호

    void Update()
    {
        var players = GameSystem.Instance.GetPlayerList();
        // 찾아오기
        InGamePlayMovement myPlayer = null;
        foreach (var player in players)
        {
            if (player.hasAuthority)
            {
                myPlayer = player;
                break;
            }
        }
        //채팅 안칠때만 사용
        if (myPlayer.isChatting == false) {
            if (Input.GetButtonDown("action") && isCCTV == false && myPlayer.CCTV_Area == true && myPlayer.isElec == false)    // 미니맵 쓸 때 응용 가능
            {
                isCCTV = true;
                CCTVUI.enabled = true;          // cctv UI 켜기
                Bright.SetActive(true);         // cctv UI 맵 밝기 켜주기
            }
            else if (Input.GetButtonDown("action") && isCCTV == true && myPlayer.CCTV_Area == true) {
                isCCTV = false;
                CCTVUI.enabled = false;         // cctv ui 끄기
                Bright.SetActive(false);        // cctv UI 맵 밝기 꺼주기
            }
        }

    } // ◆ 업데이트 괄호

    public void ElecticDown()
    {
        isCCTV = false;
        CCTVUI.enabled = false;
        Bright.SetActive(false);
    }
}
