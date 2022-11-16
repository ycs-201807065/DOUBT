using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageChatUI : MonoBehaviour {
    
    [SerializeField]
    private TMP_InputField inputField;
    // Update is called once per frame
    

    void Start() {
        MessageBoxSetting.activeMessageChat = false;
        inputField.interactable = true;
    }
    void Update() {
        // 엔터쳤을때 포커스 -> 이것때문에 자꾸 진입하는 느낌인데 나중에 확인하기!
        if (Input.GetKeyDown(KeyCode.Return)) {
            MessageBoxSetting.activeMessageChat = true;
            inputField.interactable = true; // 채팅입력 활성화(inputfield 활성화)
            inputField.ActivateInputField();
            //inputField.Select();
        }
        // esc 눌렀을때, 채팅 포커스 아닐때 풀어주기
        else if (Input.GetKeyDown(KeyCode.Escape) || inputField.isFocused == false) {
            MessageBoxSetting.activeMessageChat = false;
            inputField.interactable = false; // 채팅입력 비활성화(inputfield 비활성화)
        }

        //pageup : 투명도 올리기
        if (Input.GetKeyDown(KeyCode.PageUp)) {
            GameObject messageBoxImage = GameObject.Find("Message Box/Panel");
            Color color = messageBoxImage.GetComponent<Image>().color;
            if (color.a <= 0.75f) {
                color.a += 0.25f;
                messageBoxImage.GetComponent<Image>().color = color;
            }
        }
        //pagedown : 투명도 내리기
        else if (Input.GetKeyDown(KeyCode.PageDown)) {
            GameObject messageBoxImage = GameObject.Find("Message Box/Panel");
            Color color = messageBoxImage.GetComponent<Image>().color;
            if (color.a >= 0.5f) {
                color.a -= 0.25f;
                messageBoxImage.GetComponent<Image>().color = color;
            }
        }
    }
}
    
