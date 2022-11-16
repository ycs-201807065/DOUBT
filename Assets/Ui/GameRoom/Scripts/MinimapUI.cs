using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MinimapUI : MonoBehaviour
{
    [SerializeField]
    private Transform left;
    [SerializeField]
    private Transform right;
    [SerializeField]
    private Transform top;
    [SerializeField]
    private Transform bottom;

    [SerializeField]
    private Image minimapImage;
    [SerializeField]
    private Image minimapPlayerImage;
    [SerializeField]
    private Image minimapPlayerImage2;
    [SerializeField]
    private Image missonLocation0;
    [SerializeField]
    private Image missonLocation1;
    [SerializeField]
    private Image missonLocation2;
    [SerializeField]
    private Image missonLocation3;
    [SerializeField]
    private Image missonLocation4;
    [SerializeField]
    private Image missonLocation5;

    private PlayerMovement targetPlayer;
    private Canvas minimapUI;
    public static bool isminimap;
    public int i;
    // Start is called before the first frame update
    void Start()
    {
        isminimap = false;
        var inst = Instantiate(minimapImage.material);
        minimapImage.material = inst;
        minimapUI = GetComponentInChildren<Canvas>();


    }

    // Update is called once per frame
    void Update()
    {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players)
        {
            if (player.hasAuthority && player.playerType == EPlayerType.Researcher)
            {
                targetPlayer = player;
                i = 1;
                break;
            }
            else if (player.hasAuthority && player.playerType == EPlayerType.Infection)
            {
                targetPlayer = player;
                i = 2;
                break;
            }
        }

        if (targetPlayer != null && i == 1)
        {
            Vector2 mapArea = new Vector2(Vector3.Distance(left.position, right.position),
                Vector3.Distance(bottom.position, top.position));
            Vector2 charPos = new Vector2(Vector3.Distance(left.position, new Vector3(targetPlayer.transform
                .position.x, 0f, 0f)), Vector3.Distance(bottom.position, new Vector3(0f, targetPlayer.transform.position.y, 0f)));
            Vector2 normalPos = new Vector2(charPos.x / mapArea.x, charPos.y / mapArea.y);

            minimapPlayerImage.rectTransform.anchoredPosition = new Vector2(minimapImage.rectTransform.sizeDelta.x *
                normalPos.x, minimapImage.rectTransform.sizeDelta.y * normalPos.y);
            minimapPlayerImage.enabled = true;
            minimapPlayerImage2.enabled = false;
        }
        else if (targetPlayer != null && i == 2)
        {
            Vector2 mapArea = new Vector2(Vector3.Distance(left.position, right.position),
                Vector3.Distance(bottom.position, top.position));
            Vector2 charPos = new Vector2(Vector3.Distance(left.position, new Vector3(targetPlayer.transform
                .position.x, 0f, 0f)), Vector3.Distance(bottom.position, new Vector3(0f, targetPlayer.transform.position.y, 0f)));
            Vector2 normalPos = new Vector2(charPos.x / mapArea.x, charPos.y / mapArea.y);

            minimapPlayerImage2.rectTransform.anchoredPosition = new Vector2(minimapImage.rectTransform.sizeDelta.x *
                normalPos.x, minimapImage.rectTransform.sizeDelta.y * normalPos.y);
            minimapPlayerImage2.enabled = true;
            minimapPlayerImage.enabled = false; 
        }

        if (Input.GetButtonDown("minimap") && isminimap == false)    // 미니맵쓸때응용가능
        {
            isminimap = true;
            minimapUI.enabled = true;        // cctv UI 켜기
        }
        else if (Input.GetButtonDown("minimap") && isminimap == true)
        {
            isminimap = false;
            minimapUI.enabled = false;        // cctv ui 끄기
        }
        for (int i = 0; i < 6; i++)
        {
            if (GameSystem.Instance.missionsTransform[i].activeSelf == true)
            {
                if (i == 0) missonLocation0.enabled = true;
                else if (i == 1) missonLocation1.enabled = true;
                else if (i == 2) missonLocation2.enabled = true;
                else if (i == 3) missonLocation3.enabled = true;
                else if (i == 4) missonLocation4.enabled = true;
                else if (i == 5) missonLocation5.enabled = true;
            }
        }


    }

}
