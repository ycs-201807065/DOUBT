using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour {
    [SerializeField]
    SpriteRenderer spriteRenderer;
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            spriteRenderer.enabled = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            spriteRenderer.enabled = false;
        }
    }
}
