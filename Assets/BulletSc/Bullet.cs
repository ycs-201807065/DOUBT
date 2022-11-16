using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour {

    void Start()
    {
        if(this.tag == "Bullet")    // 연구원 총알
        {
            Destroy(gameObject, 2f);
        }
        else if (this.tag == "Infection_Bullet")    // 감염체 총알
        {
            Destroy(gameObject, 0.1f);    //총알  사거리 설정
        }

    }
    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Wall" && this.tag == "Bullet") {
            Destroy(gameObject);
        }
        if(collision.tag == "Player" && this.tag == "Infection_Bullet") {
            InGamePlayMovement test = collision.gameObject.GetComponent<InGamePlayMovement>();
            if (test.playerType == EPlayerType.Researcher) {
                Destroy(gameObject);
                return;

            }
        }
        else if (collision.tag == "Player" && this.tag == "Bullet") {
            InGamePlayMovement test = collision.gameObject.GetComponent<InGamePlayMovement>();
            if (test.playerType == EPlayerType.Infection) {
                Destroy(gameObject);
                return;
            }
        }
    }

}
