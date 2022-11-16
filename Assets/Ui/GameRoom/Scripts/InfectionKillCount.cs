using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InfectionKillCount : NetworkBehaviour
{
    public static InfectionKillCount Instance;
    [SyncVar]
    public int killNum;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start() {
        killNum = 0;
    }
    

    // 감염체의 킬 수를 변경하는 함수
    [Command(requiresAuthority = false)]
    public void CmdkillCount()
    {
        killNum += 1;
        return;
    }
}
