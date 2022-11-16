using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ObjectShadowBottom : MonoBehaviour
{
    public ShadowCaster2D shadow;

    private void OnTriggerEnter2D(Collider2D other) {
        shadow = this.GetComponent<ShadowCaster2D>();
        string name = "";
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players) {
            if(player.hasAuthority) {
                name = player.nickname;
            } else {
                //player.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
        if(name == other.GetComponent<InGamePlayMovement>().nickname) {
            //shadow.selfShadows = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        shadow = this.GetComponent<ShadowCaster2D>();
        string name = "";
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players) {
            if(player.hasAuthority) {
                name = player.nickname;
            } else {
                //player.GetComponent<SpriteRenderer>().sortingOrder = 100;
            }
        }
        if(name == other.GetComponent<InGamePlayMovement>().nickname) {
            //shadow.selfShadows = false;
        }
    }
}
