using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultCharacter : MonoBehaviour
{
    [SerializeField]
    private Image character;
    
    [SerializeField]
    private Text nickname;

    [SerializeField]
    private Text status;

    // 인트로 캐릭터 정보 설정
    public void SetResultCharacter(string nick, string stat)
    {
        var matInst = Instantiate(character.material);
        character.material = matInst;

        nickname.text = nick;
        status.text = stat;
    }
}
