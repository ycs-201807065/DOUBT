using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Net;
using System.Net.Sockets;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField]
    private List<Image> crewImgs;
    [SerializeField]
    private List<Button> escTimeButtons;
    [SerializeField]
    private List<Image> maxPlayerCountButtons;

    private CreateGameRoomData gameRoomData;
    
    // Start is called before the first frame update
    void Start()
    {
        //필요없으면 지우기
        for (int i = 0; i < crewImgs.Count; i++) {
            Material materialInstance = Instantiate(crewImgs[i].material);
            crewImgs[i].material = materialInstance;
        }
        gameRoomData = new CreateGameRoomData() { escTime = 3, maxPlayerCount = 4 };
        InfestedCivCountUpdate();
    }

    public void EscTrainTimeUpdate(int count) {
        //열차 도착 시간
        gameRoomData.escTime = count;
        Debug.Log(gameRoomData.escTime);  //잘 되나 로그 테스트
    }

    public void MaxPlayerUpdate(int count) {
        //최대 플레이어 숫자
        gameRoomData.maxPlayerCount = count;
        InfestedCivCountUpdate();
    }

    private void InfestedCivCountUpdate() {
        //이제 감염체 1명 고정이라 딱 1개만 출력하게 바꿈
        //최대 플레이어 수에 따라 감염자 보이기 생성
        int civCount = gameRoomData.maxPlayerCount;
        Debug.Log(civCount);  //잘 되나 로그 테스트
        //첫 시작 다 가려주기
        for (int i = 0; i < 6; i++) {
            crewImgs[i].gameObject.SetActive(false);
        }

        //보여주기
        if (civCount == 6) {
            for (int i = 0; i < 6; i++) {
                crewImgs[i].gameObject.SetActive(true);
            }
        }
        else if (civCount == 5) {
            for (int i = 0; i < 5; i++) {
                crewImgs[i].gameObject.SetActive(true);
            }
        }
        else if (civCount == 4) {
            for (int i = 0; i < 4; i++) {
                crewImgs[i].gameObject.SetActive(true);
            }
        }
    }

    public void CeateRoom() {
        //Server server = new Server();
        //Client client = new Client();
        var manager = NetworkRoomManager.singleton as MafiaRoomManager;
        manager.trainTime = gameRoomData.escTime;
        manager.playerCount = gameRoomData.maxPlayerCount;
        manager.StartHost();
        //server.ServerCreate();
        //client.ConnectToServer();
    }
}

public class CreateGameRoomData {
    public int escTime;
    public int maxPlayerCount;
}
