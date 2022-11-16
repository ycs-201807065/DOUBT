using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Canal : NetworkBehaviour
{
    //[SyncVar(hook = nameof(SetCanalHp_Hook))]
    [SyncVar]
    private int hp;
    /*
    public void SetCanalHp_Hook(int _, int value) {
        hp = value;
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        hp = 8;
    }

void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.tag == "Bullet")
        {  
            if (isServer){
                hp--;
                if(hp <= 0){
                    Destroy(gameObject);
                }
            }
        }
    }
        
}
