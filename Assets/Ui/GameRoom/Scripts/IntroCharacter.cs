using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCharacter : MonoBehaviour
{
    [SerializeField]
    private Image character;
    
    [SerializeField]
    private Text nickname;

    [SerializeField]
    private Sprite InfectionImage;

    // 인트로 캐릭터 정보 설정
    public void SetIntroCharacter(string nick)
    {
        var matInst = Instantiate(character.material);
        character.material = matInst;

        nickname.text = nick;
    }

    // 감염체 캐릭터 이미지로 변경
    public void ChangeInfectionCharacter()
    {
        character.GetComponent<Image>().sprite = InfectionImage;
    }
}
