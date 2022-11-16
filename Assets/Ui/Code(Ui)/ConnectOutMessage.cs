using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectOutMessage : MonoBehaviour
{
    public static bool messageCheck;
    public GameObject messageBox;

    private void Update() {
        if(messageCheck) {
            messageBox.gameObject.SetActive(true);
        } else {
            messageBox.gameObject.SetActive(false);
        }
    }

    public void onClickMessageOKButton() {
        messageCheck = false;
    }
}
