using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkButtonScript : NetworkBehaviour
{
    public void OnClickExitButton()
    {
        var manager = NetworkManager.singleton as MafiaRoomManager;
        if(!isServer){
            ConnectOutMessage.messageCheck = false;
            manager.StopClient();
        } else {
            ConnectOutMessage.messageCheck = false;
            manager.StopHost();
        }
    }
}
