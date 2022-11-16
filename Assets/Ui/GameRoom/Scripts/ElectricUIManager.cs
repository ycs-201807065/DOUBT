using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ElectricUIManager : NetworkBehaviour
{
    public static ElectricUIManager Instance;
    public List<GameObject> electricImages = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        ElectricActive();
    }

    private void ElectricActive()
    {
        var players = GameSystem.Instance.GetPlayerList();
        foreach (var player in players)
        {
            // 전기실 고장
            if (player.hasAuthority && player.isElec)
            {
                CCTV.Instance.ElecticDown();
                int looking = player.GetShotFlag();
                Quaternion rotation;
                
                if (player.playerType != EPlayerType.Infection)
                {
                    Electric.Instance.PointLight.pointLightInnerRadius = 0.25f;
                    Electric.Instance.PointLight.pointLightOuterRadius = 0.3f;
                    InfectionLight.Instance.freeForm.intensity = 0.0f;
                    foreach (var electric in electricImages)
                    {
                        electric.SetActive(false);
                    }
                    break;
                }
                else
                {
                    Electric.Instance.PointLight.pointLightInnerRadius = 0.0f;
                    Electric.Instance.PointLight.pointLightOuterRadius = 0.0f;
                    InfectionLight.Instance.freeForm.intensity = 0.5f;

                    if (looking == 1)
                    {
                        // 오른쪽
                        rotation = Quaternion.Euler(0, 0, 270);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }
                    else if (looking == 2)
                    {
                        // 왼쪽
                        rotation = Quaternion.Euler(0, 0, 90);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }
                    else if (looking == 3)
                    {
                        // 위쪽
                        rotation = Quaternion.Euler(0, 0, 0);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }
                    else if (looking == 4)
                    {
                        // 아래쪽
                        rotation = Quaternion.Euler(0, 0, 180);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }

                    foreach (var electric in electricImages)
                    {
                        electric.SetActive(false);
                    }
                    break;
                }
            }

            // 평시 상태 및 전기실 수리
            if (player.hasAuthority && player.isElec == false)
            {
                int looking = player.GetShotFlag();
                Quaternion rotation;
                
                if (player.playerType != EPlayerType.Infection)
                {
                    Electric.Instance.PointLight.pointLightInnerRadius = 1.05f;
                    Electric.Instance.PointLight.pointLightOuterRadius = 1.1f;
                    InfectionLight.Instance.freeForm.intensity = 0.0f;
                    foreach (var electric in electricImages)
                    {
                        electric.SetActive(true);
                    }
                    break;
                }
                else
                {
                    Electric.Instance.PointLight.pointLightInnerRadius = 0.0f;
                    Electric.Instance.PointLight.pointLightOuterRadius = 0.0f;
                    InfectionLight.Instance.freeForm.intensity = 0.5f;

                    if (looking == 1)
                    {
                        // 오른쪽
                        rotation = Quaternion.Euler(0, 0, 270);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }
                    else if (looking == 2)
                    {
                        // 왼쪽
                        rotation = Quaternion.Euler(0, 0, 90);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }
                    else if (looking == 3)
                    {
                        // 위쪽
                        rotation = Quaternion.Euler(0, 0, 0);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }
                    else if (looking == 4)
                    {
                        // 아래쪽
                        rotation = Quaternion.Euler(0, 0, 180);
                        InfectionLight.Instance.freeForm.transform.rotation = rotation;
                    }

                    foreach (var electric in electricImages)
                    {
                        electric.SetActive(true);
                    }
                    break;
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdElecticActive()
    {
        if (isServer)
        {
            RpcElectricActive();
        }
    }

    [ClientRpc]
    private void RpcElectricActive()
    {
        var players = GameSystem.Instance.GetPlayerList();
        if (players[0].isElec == false)
        {
            foreach (var player in players)
            {
                player.isElec = true;
            }
            ElectricBroken();
        } else {
            foreach (var player in players)
            {
                player.isElec = false;
            }
            ElectricRepair();
        }

    }

    private void ElectricBroken()
    {
        NoticeUIManager.Instance.TitleSet("전기실 고장", "전기실 고장");
        NoticeUIManager.Instance.NoticeSet("시야가 감소합니다", "연구원의 시야가 감소합니다");
        NoticeUIManager.Instance.Open();
    }

    private void ElectricRepair()
    {
        NoticeUIManager.Instance.TitleSet("전기실 수리", "전기실 수리");
        NoticeUIManager.Instance.NoticeSet("시야가 복구됩니다", "연구원의 시야가 복구됩니다");
        NoticeUIManager.Instance.Open();
    }
}
