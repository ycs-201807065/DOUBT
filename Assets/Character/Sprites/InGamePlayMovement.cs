using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class InGamePlayMovement : PlayerMovement
{
    [SyncVar]
    public bool Train_Area;
    [SyncVar]
    public bool Medical_Area;
    public bool CCTV_Area;
    [SyncVar]
    public bool Electric_Area;
    [SyncVar]
    public bool Mission_Area;

    [SyncVar]
    public bool Canal1_Area;
    [SyncVar]
    public bool Canal2_Area;

    private int cameraIndex;
    [SyncVar]
    public bool escape;
    [SyncVar]
    public int trainSector = 0;
    [SyncVar]
    public int missionSector = 0;
    [SyncVar]
    public bool isElec = false;
    private bool isBtnClick;  //죽은 카메라 버튼 클릭
    private bool chatActive = true; // 채팅 활성화 여부
    public List<GameObject> playerStatusImages = new List<GameObject>();

    private bool canalDelay; //커널 딜레이
    public float missionClearTimer;  //미션 수행중인 시간

    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
        // 텔레포트되면 캐릭터 이동속도 지정, 확인하기 쉽게 1로둠
        /// 나중에 텔레포트 지점 바꿔주기
    }
    public void InfectionTeleport(Vector3 position) {
        transform.position = position;
        // 텔레포트되면 캐릭터 이동속도 지정, 확인하기 쉽게 1로둠
        /// 나중에 텔레포트 지점 바꿔주기
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //IsMoveable = true;

        if (hasAuthority)
        {
            var myRoomPlayer = MafiaRoomPlayer.MyRoomPlayer; 
            CmdSetPlayerCharacter(myRoomPlayer.nickname, myRoomPlayer.netId);
        }

        cameraIndex = -1;  // 0부터하면 함수 실행시 바로 1되서 -1부터했는데 함수 순서 바꿀거면 이것도 바꾸기
        GameSystem.Instance.AddPlayer(this);
        isHit = false;
        isMoving = true;
        startGame = false;
    }

    [Command(requiresAuthority = false)]
    private void CmdSetPlayerCharacter(string nickname, uint netId)
    {
        hp = 5;
        maxHp = hp;
        bulletAmmo = 5;
        this.nickname = nickname;
        ownerNetId = netId;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Train_Area" && playerType == EPlayerType.Researcher)
        {
            Train_Area = true;
        }

        if (other.tag == "Out_Area" && playerType == EPlayerType.Researcher)
        {
            Train_Area = false;
            Medical_Area = false;
        }
        //시민 -> 감염체
        if (other.tag == "Bullet" && playerType == EPlayerType.Infection)
        {  // 곂친 상대의 태그가 Bullet 인 경우 처리
            BulletAttack bullet = this.gameObject.AddComponent<BulletAttack>();
            bool hit;
            BulletAttack.victim = nickname;
            hit = bullet.Hit();
            //if문에서 hit 뺌
            if (isHit == false)
            {
                isHit = true;
                Debug.Log("시민 -> 감염체");
                StartCoroutine("ChangeSpeed");
            }

        }
        //감염체 -> 시민
        else if (other.tag == "Infection_Bullet" && playerType == EPlayerType.Researcher)
        {  // 곂친 상대의 태그가 Infection_Bullet 인 경우 처리
            // 감염체가 감염체 총알(Infection_Bullet)로 공격할 때 데미지 계산
            BulletAttack bullet = this.gameObject.AddComponent<BulletAttack>();
            bool hit;
            BulletAttack.victim = nickname;
            hit = bullet.Hit();

            //if문에서 hit 뺌
            if (isHit == false)
            {
                isHit = true;
                Debug.Log("감염체 -> 시민");
                StartCoroutine("ChangeSpeed");
                if (isServer)
                {
                    if (hit)
                    {
                        if (isServer)
                        {
                            hp--;
                            //hp =0;    // 테스트
                        }
                    }

                    if (hp <= 0 && playerType != EPlayerType.Ghost)
                    {
                        InfectionKillCount.Instance.CmdkillCount();
                        tag = "DeadPlayer"; // 태그 바꿔주기
                        playerType = EPlayerType.Ghost;
                        isMoving = false;
                        SliderViewOut();
                        RpcDead();
                    }
                }
            }
        }

        if (hasAuthority && other.tag == "Mission" && playerType == EPlayerType.Researcher) {
            Mission_Area = true;
            MissionSlider.SetActive(true);
        }

        if (other.tag == "CCTV" && playerType != EPlayerType.Ghost)
        {
            CCTV_Area = true;
        }

        if (other.tag == "Canal1" && playerType == EPlayerType.Infection)
        {
            Canal1_Area = true;
        }
        if (other.tag == "Canal2" && playerType == EPlayerType.Infection)
        {
            Canal2_Area = true;
        }


        if (other.tag == "Electric" && (playerType == EPlayerType.Researcher || playerType == EPlayerType.Infection))
        {
            Electric_Area = true;
            if(hasAuthority && playerType == EPlayerType.Researcher && isElec == true) {
                MissionSlider.SetActive(true);
            }
            
        }
        if (other.tag == "Creep") {
            isCreep = true;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "CCTV" && playerType != EPlayerType.Ghost)
        {
            CCTV_Area = false;
        }

        if (other.tag == "Electric" && (playerType == EPlayerType.Researcher || playerType == EPlayerType.Infection))
        {
            Electric_Area = false;
            SliderViewOut();
        }

        if (hasAuthority && other.tag == "Mission" && playerType == EPlayerType.Researcher) {
            Mission_Area = false;
            SliderViewOut();
        }
        if (other.tag == "Creep") {
            isCreep = false;
        }
        if (other.tag == "Canal1" && playerType == EPlayerType.Infection)
        {
            Canal1_Area = false;
        }
        if (other.tag == "Canal2" && playerType == EPlayerType.Infection)
        {
            Canal2_Area = false;
        }
    }

    void Update()
    {
        CanalSystem();
        OnLookCCTV();
        ResearcherEscape();
        WhoFindPlayer();
        PlayerStatus();
        MissionSliderView();
        MessageBoxActive();
    }
    private void SliderViewOut() {
        MissionSlider.SetActive(false);
        MissionSlider.GetComponent<Slider>().value = 0;
        missionClearTimer = 0;
        MissionText.text = "";
    }

    private void CanalSystem()
    {
        
        if (Input.GetButtonDown("action") && Canal1_Area && GameObject.Find("Canal 2(Clone)") != null && canalDelay == false)
        {
            canalDelay = true;
            transform.position = canalTransform2;
            StartCoroutine("CanalDelayCoroutine");
            Canal1_Area=false;
            Canal2_Area=true;
        }
        else if (Input.GetButtonDown("action") && Canal2_Area&& GameObject.Find("Canal 1(Clone)") != null && canalDelay == false) 
        {
            canalDelay = true;
            transform.position = canalTransform1;
            StartCoroutine("CanalDelayCoroutine");
            Canal1_Area=true;
            Canal2_Area=false;
        }
    }

    public IEnumerator CanalDelayCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        canalDelay = false;
    }

    public void OnLookCCTV()
    {
        if (CCTV.isCCTV)
        {
            isWatchingCCTV = true;
        }
        else
        {
            isWatchingCCTV = false;
            return;
        }
    }

    public void MissionSliderView() {

        if (Mission_Area == true) {
            if (Input.GetButtonDown("action") && hasAuthority) {
                //60~70% 사이 스타캐치 구간 min(62%), max(70%)
                float minEventTimer = Mission.mInstance.missionNeedTimer * 0.62f;  // 이벤트 지점 처음
                float maxEventTimer = Mission.mInstance.missionNeedTimer * 0.7f;  // 이벤트 지점 이후
                //스타캐치 성공, 실패
                if (missionClearTimer >= minEventTimer && missionClearTimer <= maxEventTimer) {
                    missionClearTimer += 3.5f;
                }
                else {
                    missionClearTimer -= 10.0f;
                    if(missionClearTimer <= 0) {
                        missionClearTimer = 0f;
                    }
                }
            }
            //MissionText.text = missionClearTimer.ToString() + " / " + Mission.mInstance.missionNeedTimer.ToString();  //시간 보이는거 1 / 10
            MissionSlider.GetComponent<Slider>().value = missionClearTimer / Mission.mInstance.missionNeedTimer;
        }
        if (Electric_Area == true && isElec == true && playerType == EPlayerType.Researcher) {
            if (Input.GetButtonDown("action") && hasAuthority) {
                //60~70% 사이 스타캐치 구간 min(62%), max(70%)
                float minEventTimer = Electric.Instance.electricNeedNumber * 0.62f;  // 이벤트 지점 처음
                float maxEventTimer = Electric.Instance.electricNeedNumber * 0.7f;  // 이벤트 지점 이후
                //스타캐치 성공, 실패
                if (missionClearTimer >= minEventTimer && missionClearTimer <= maxEventTimer) {
                    missionClearTimer += 3.0f;
                }
                else {
                    missionClearTimer -= 8.0f;
                    if (missionClearTimer <= 0) {
                        missionClearTimer = 0f;
                    }
                }
            }
            //MissionText.text = missionClearTimer.ToString() + " / " + Electric.Instance.electricNeedNumber.ToString();  //시간 보이는거 1 / 10
            MissionSlider.GetComponent<Slider>().value = missionClearTimer / Electric.Instance.electricNeedNumber;
        }
        else if (Electric_Area == true && isElec == false && playerType == EPlayerType.Researcher) {
            SliderViewOut();
        }

    }

    public void ResearcherEscape() // 플레이어 탈출 여부
    {
        if (Input.GetButtonDown("action") && playerType == EPlayerType.Researcher && Train_Area == true && IngameUIManager.Instance.escapeActive == true && hasAuthority == true)
        {
            CmdPlayerUpdate(this);
            return;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayerUpdate(InGamePlayMovement player)
    {
        player.tag = "EscapePlayer";
        player.playerType = EPlayerType.WinResearcher;
        player.escape = true;
        player.isMoving = false;
        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        player.GetComponent<SpriteRenderer>().color = color;
        player.hpText.enabled = false;
        player.ReloadText.enabled = false;
        player.nicknameText.enabled = false;
        RpcPlayerUpdate(player);
        return;
    }

    [ClientRpc]
    public void RpcPlayerUpdate(InGamePlayMovement player)
    {
        player.tag = "EscapePlayer";
        player.playerType = EPlayerType.WinResearcher;
        player.escape = true;
        player.isMoving = false;
        Color color = player.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        player.GetComponent<SpriteRenderer>().color = color;
        player.hpText.enabled = false;
        player.ReloadText.enabled = false;
        player.nicknameText.enabled = false;
        return;
    }

    public void WhoFindPlayer()
    {
        //일반시민이 아니라면(감염체, 죽은사람, 승리 플레이어) 넘겨주는 기능
        ///카메라 인덱스 플레이어 직업 확인
        ///시민이 아니라면(감염체, 죽은사람, 승리 플레이어) 카메라 인덱스 바로 올려주기
        if ((this.playerType == EPlayerType.Ghost || this.playerType == EPlayerType.WinResearcher) && Input.GetButtonDown("deadview") && hasAuthority)
        {
            var findPlayers = GameSystem.Instance.GetPlayerList();
            while (true)
            {
                //카메라 인덱스 선 증가
                cameraIndex++;
                //카메라 인덱스가 범위 넘어가면 1P 부터 다시 잡아주기
                Debug.Log("카메라 : " + cameraIndex + " / 플레이어 카운트 : " + findPlayers.Count);
                if (cameraIndex >= findPlayers.Count)
                {
                    cameraIndex = 0;
                }
                //현재 카메라 인덱스 플레이어가 시민일 경우에만 카메라 적용시켜주기
                if (findPlayers[cameraIndex].playerType == EPlayerType.Researcher)
                {
                    InGamePlayMovement findPlayer = findPlayers[cameraIndex];
                    Camera cam = Camera.main;
                    //카메라 보여주기
                    cam.transform.SetParent(findPlayer.transform);
                    cam.transform.localPosition = new Vector3(0f, 0f, -10f);
                    break;
                }

            }
        }
    }

    public void PlayerStatus() {
        if (isHit == true) {
            if (playerType == EPlayerType.Infection) {
                playerStatusImages[0].SetActive(true);
            }
            else if (playerType == EPlayerType.Researcher) {
                playerStatusImages[1].SetActive(true);
            }
        }
        else {
            foreach (var playerStatusImage in playerStatusImages) {
                playerStatusImage.SetActive(false);
            }
        }
    }

    private void MessageBoxActive()
    {
        if(hasAuthority && Input.GetButtonDown("chat") && chatActive) {
            chatActive = false;
            MessageChatController.Instance.MessageBoxSetting(false);
        } else if(hasAuthority && Input.GetButtonDown("chat") && chatActive == false) {
            chatActive = true;
            MessageChatController.Instance.MessageBoxSetting(true);
        }
    }

    public IEnumerator ChangeSpeed()
    {
        yield return new WaitForSeconds(2.5f);
        isHit = false;
    }

    [ClientRpc]
    private void RpcDead()
    {
        anim.SetBool("isGhost", true);
    }
}