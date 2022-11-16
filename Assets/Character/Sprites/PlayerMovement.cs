using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

// 플레이어가 연구원인지 감염체인지 구분할 용도의 열거형
public enum EPlayerType // 성운 ingameplaymovement에서 옮김
{
    // 연구원, 탈출한 연구원, 감염체, 사망한 연구원
    Researcher,
    WinResearcher,
    Infection,
    Ghost
}

public class PlayerMovement : NetworkBehaviour {

    [SerializeField]
    public Text MissionText;  //임시 텍스트
    [SerializeField]
    public GameObject MissionSlider;  //미션 슬라이드창

    //================= 감염체 공격모션 관련 변수들
    [SerializeField]
    public GameObject Infection_Attack_Right;
    [SerializeField]
    public GameObject Infection_Attack_Left;
    [SerializeField]
    public GameObject Infection_Attack_Up;
    [SerializeField]
    public GameObject Infection_Attack_Down;
    public GameObject Infection_Attack_Direction;   // 감염체 공격방향 저장 변수
    [SyncVar]
    public bool isTempInfectionAttack; // 공격모션 끄기 
    //public bool check = true;   //StartCoroutine(WaitForIt()); 함수 사용 시 사용
    //=================

    public Rigidbody2D rig;
    public Rigidbody2D creepRig;
    public Animator anim;  // 애니메이션 관련 정보

    private float moveX;
    private float moveY;
    public bool isMoving;  // 이동 가능한지 확인 
    public bool isChatting;    // 채팅중인지 확인
    public bool isWatchingCCTV; // CCTV 보는중인지 확인
    public bool isLife; // 살아있는지 확인
    public bool startGame;  // 게임 시작상태인지 확인
    [SyncVar]
    public bool isHit;  //뭔가 맞았는지 확인
    [SyncVar]
    public bool isCreep;  //점막에 있는지 확인

    //감염체 능력 부여
    [SyncVar]
    public int randomSkill;  //스킬 랜덤
    private GameObject creepObj;  //점막 오브젝트
    public GameObject CreepPrefab;  // 점막 프리펩
    public int skillMaxCount;  //스킬 횟수
    private bool isSafeSkill; //스킬 안전장치
    //커널
    public GameObject canalObj1;  //커널 오브젝트1
    public GameObject canalObj2;  //커널 오브젝트2
    public Vector3 canalTransform1;  //커널 트랜스폼1
    public Vector3 canalTransform2;  //커널 트랜스폼2
    public GameObject CanalPrefab1;  // 커널 프리펩1
    public GameObject CanalPrefab2;  // 커널 프리펩2
    private bool isCanalFirst;


    [SyncVar]
    public EPlayerType playerType;  // 성운 ingameplaymovement에서 옮김

    [SerializeField]
    private Transform playerTransform;

    [SyncVar]
    public float moveSpeed;

    [SerializeField]
    private float characterSize = 0.75f;  //0725

    [SerializeField]
    private float cameraSize = 2.5f;    //카메라

    [SyncVar]
    private int shotFlag;  //방향 플래그 변수
    private GameObject bullet;
    public GameObject BulletPrefab;
    public GameObject InBulletPrefab;
    public int bulletAmmo;

    [SerializeField]
    public Text ReloadText;
    private float shotSpeed;  // 총알 속도 조정
    public int maxHp;
    public int playerHp;
    [SyncVar(hook = nameof(SetHp_Hook))]
    public int hp; // 플레이어 체력
    [SerializeField]
    public Text hpText;
    public void SetHp_Hook(int _, int value) {
        playerHp = hp;
    }

    private bool SpawnBullet;   // 총알 발사 가능 여부
    private bool ReloadBullet;
    public string attacker;    // 공격자

    // 이름 관련
    [SyncVar(hook = nameof(SetOwnerNetId_Hook))]
    public uint ownerNetId;
    public void SetOwnerNetId_Hook(uint _, uint newOwnerId) {
        var players = FindObjectsOfType<MafiaRoomPlayer>();
        foreach (var player in players) {
            if (newOwnerId == player.netId) {
                player.playerCharacter = this;
                break;
            }
        }
    }

    // 이름 관련 (닉네임 동기화 해제 11/09)
    //[SyncVar(hook = nameof(SetNickname_Hook))]
    [SyncVar]
    public string nickname;
    public Text nicknameText;

    /*
    public void SetNickname_Hook(string _, string value)
    {
        nicknameText.text = nicknameTest;
    }
    */
    void Awake() {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public virtual void Start() {
        // 변수 초기 값 설정
        bulletAmmo = 5;  // 확인용
        shotFlag = 0;
        ReloadBullet = true;
        nicknameText.text = "";
        ReloadText.text = "";
        hpText.text = "";
        MissionText.text = "";
        hp = 5;
        playerHp = hp;
        maxHp = hp;
        isTempInfectionAttack = false;
        startGame = true;
        // 카메라 조정 코드
        if (hasAuthority) {
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = cameraSize;
        }
    }

    void FixedUpdate() {
        Move();
        Fire();
        Skill();
        OnClickMessageButton();
        AnimationSetting();
        MessageChatUpdate();
    }

    // 이동 & 애니메이션 함수
    void Move() {
        // 텍스트 관리
        if (hasAuthority) {
            if (playerHp <= 0) {
                isLife = false;
            }
            //기본값
            ReloadText.text = "";
            hpText.text = "";
            nicknameText.text = nickname;  //동기화 안되는 닉네임

            //살아있으면 제대로 보이기
            if (playerType == EPlayerType.Researcher)   // 성운,연구원일 때 총알 개수 보여주기, 감염체는 총알을 안 쓰기 때문에 총알 개수 지움
            {
                ReloadText.text = "Ammo : " + bulletAmmo.ToString();
                hpText.text = "Hp : " + playerHp.ToString(); //감염체는 기본 체력 안보여줌
            }
            if (playerType == EPlayerType.Infection && randomSkill > 0) {
                ReloadText.text = "SkillCount : " + skillMaxCount.ToString();  //감염체 스킬횟수
            }
            

        }

        if (hasAuthority && isMoving && startGame && isLife && isChatting == false && isWatchingCCTV == false) {
            // 자꾸 바꿔주면 과부화 올것같은데 체력까지 되면 if로 빼기
            /// 바뀐 이동 시작
            Vector3 dir = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f), 1f);
            if (dir.x < 0f) {
                transform.localScale = new Vector3(-characterSize, characterSize, 1f);
            }
            else if (dir.x > 0f) {
                transform.localScale = new Vector3(characterSize, characterSize, 1f);
            }

            moveSpeed = GameRuleStore.grInstance.playerSpeed;  // 이동속도 받아오기

            if (playerType == EPlayerType.Infection) {
                if (isHit) {
                    moveSpeed *= 0.5f;  //감염체가 맞으면 50% 감속
                    Debug.Log("감염체 느려짐");
                }
                else {
                    moveSpeed *= 1.1f;  //감염체 기본 이동 10% 가속
                }
                if (isCreep) {
                    moveSpeed *= 1.3f;  //감염체 점막 위 30% 가속
                }
            }
            else if (playerType == EPlayerType.Researcher) {
                if (isHit) {
                    moveSpeed *= 1.5f; //시민이 맞으면 50% 가속
                    Debug.Log("시민 빨라짐");
                }
                if (isCreep) {
                    moveSpeed *= 0.8f;  //시민 점막 위 20% 감속
                }
            }

            transform.position += dir * moveSpeed * Time.deltaTime;
            /// 바뀐 이동 끝

            // 애니메이션 세팅
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");

            if (anim.GetInteger("hAxisRaw") != moveX) { // 좌, 우 이동 시
                anim.SetBool("isChange", true);
                anim.SetInteger("hAxisRaw", (int)moveX);
            }
            else if (anim.GetInteger("vAxisRaw") != moveY) { // 상, 하 이동 시
                anim.SetBool("isChange", true);
                anim.SetInteger("vAxisRaw", (int)moveY);
            }
            else {
                anim.SetBool("isChange", false);
            }
            // 애니메이션 세팅 끝
        }
        // 닉네임 방향전환
        if (transform.localScale.x < 0) {
            nicknameText.transform.localScale = new Vector3(-1f, 1f, 1f);
            hpText.transform.localScale = new Vector3(-1f, 1f, 1f);
            ReloadText.transform.localScale = new Vector3(-0.5f, 0.5f, 0.51f);
            //MissionText.transform.localScale = new Vector3(-0.5f, 0.5f, 0.51f);
            MissionSlider.transform.localScale = new Vector3(-0.5f, 0.5f, 0.51f);
        }
        else if (transform.localScale.x > 0) {
            nicknameText.transform.localScale = new Vector3(1f, 1f, 1f);
            hpText.transform.localScale = new Vector3(1f, 1f, 1f);
            ReloadText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //MissionText.transform.localScale = new Vector3(0.5f, 0.5f, 0.51f);
            MissionSlider.transform.localScale = new Vector3(0.5f, 0.5f, 0.51f);
        }
    }

    // 사격 함수
    void Fire() {
        // 보는 방향이 정확하지 못함(너무 세세하게 shotFlag가 수정되서), 버튼을 천천하고 정확히 1개씩만 눌러서 방향 확인해야함(일단 나가는거에 의의맞춤)
        // 누르면 쏘게하기
        // Input.GetButton("Fire1")
        // 총알 발사 방향
        if (moveX > 0 && moveY == 0) {
            // 오른쪽
            shotFlag = 1;
        }
        else if (moveX < 0 && moveY == 0) {
            // 왼쪽
            shotFlag = 2;
        }
        else if (moveX == 0 && moveY > 0) {
            // 위쪽
            shotFlag = 3;
        }
        else if (moveX == 0 && moveY < 0) {
            // 아래쪽
            shotFlag = 4;
        }

        // 컨트롤 나가면 총알 나감
        if (hasAuthority && Input.GetButton("Fire1") && shotFlag > 0 && ReloadBullet && isMoving && startGame && isLife && bulletAmmo > 0 && isChatting == false && isWatchingCCTV == false) {
            ReloadBullet = false;

            if (playerType == EPlayerType.Researcher) {
                bulletAmmo--;
            }
            InfectionAttackFunction(GetShotFlag());
            //isTempInfectionAttack = true;
            StartCoroutine("ReloadBulletTimerCoroutine");
            var manager = NetworkRoomManager.singleton as MafiaRoomManager;
            if (manager.mode == Mirror.NetworkManagerMode.Host) {
                CmdShotUpdate(GetShotFlag(), 1);
            }
            else {
                CmdShotUpdate(GetShotFlag(), 0);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdShotUpdate(int flag, int checking) {
        var roomSlots = (NetworkManager.singleton as MafiaRoomManager).roomSlots;
        foreach (var roomPlayer in roomSlots) {
            Debug.Log("RpcShotUpdate 사격 들어옴 / owner:" + ownerNetId);
            var mafiaRoomPlayer = roomPlayer as MafiaRoomPlayer;
            if (roomPlayer.netId == ownerNetId) {
                //mafiaRoomPlayer.playerCharacter.curShotDelay = 0;  // 찾아서 총알 재장전 해야함
                Debug.Log("roomPlayer.netId == ownerNetId 사격 들어옴 / " + roomPlayer.netId);
                if (roomPlayer.index == 0 && checking == 0) { // 클라이언트가 발사 시 호스트는 처리 안함
                    Debug.Log("roomPlayer.index == 0 && checking == 0 사격 들어옴");
                    SpawnBullet = false;
                    continue;
                }

                playerTransform = mafiaRoomPlayer.playerCharacter.transform;
                SpawnBullet = true;
                BulletAttack.attacker = nicknameText.text;
                break;
            }
        }

        if (SpawnBullet) {
            if (playerType == EPlayerType.Researcher) {
                shotSpeed = 300 * 0.7f;
                float rotationImage = 0f;
                if (flag == 2) {
                    rotationImage = 180.0f;
                }
                else if (flag == 3) {
                    rotationImage = 90.0f;
                }
                else if (flag == 4) {
                    rotationImage = 270.0f;
                }
                bullet = Instantiate(BulletPrefab, playerTransform.position, Quaternion.Euler(0, 0, rotationImage));
            }
            else if (playerType == EPlayerType.Infection) {
                shotSpeed = 1000 * 0.2f;  // 감염체일 경우 총알 스피드 설정
                bullet = Instantiate(InBulletPrefab, playerTransform.position, playerTransform.rotation);
            }

            rig = bullet.GetComponent<Rigidbody2D>();
            NetworkServer.Spawn(bullet);  // 총알 스폰
            attacker = nicknameText.text;
        }
        else {
            return;
        }

        if (flag == 1) {
            rig.AddForce(Vector2.right * shotSpeed, ForceMode2D.Force);
        }
        else if (flag == 2) {
            rig.AddForce(Vector2.left * shotSpeed, ForceMode2D.Force);
        }
        else if (flag == 3) {
            rig.AddForce(Vector2.up * shotSpeed, ForceMode2D.Force);
        }
        else if (flag == 4) {
            rig.AddForce(Vector2.down * shotSpeed, ForceMode2D.Force);
        }
    }

    void Skill() {
        if (hasAuthority && Input.GetButton("skill") && isChatting == false && isWatchingCCTV == false && playerType == EPlayerType.Infection && skillMaxCount > 0 && isSafeSkill == false) {
            isSafeSkill = true;
            skillMaxCount--;
            if(randomSkill == 3 && skillMaxCount == 1){
                canalTransform1 = this.transform.position;
            }
            else if(randomSkill == 3 && skillMaxCount == 0){
                canalTransform2 = this.transform.position;
            }
            CmdSkillUpdate();
        }
    }

    public void GetSkill() {
        //randomSkill = 1;
        if (randomSkill == 1) {
            //점막 변이 4회
            skillMaxCount = 4;
        }
        else if (randomSkill == 2) {
            //신체 변형은 3회
            skillMaxCount = 3;
        }
        else if (randomSkill == 3) {
            //커널은 2회
            skillMaxCount = 2;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSkillUpdate() {
        if (isServer) {
            RpcSkillUpdate();
        }
    }

    [ClientRpc]
    public void RpcSkillUpdate() {
        if (randomSkill == 1) {
            if(isServer) {
                creepObj = Instantiate(CreepPrefab, transform.position, transform.rotation);
                NetworkServer.Spawn(creepObj);  // 점막 스폰
            }
            StartCoroutine("CreepDelayCoroutine");
        }
        else if (randomSkill == 2) {
            PlayerFaceChange(1);
        }
        else if (randomSkill == 3) {
            if(isServer) {
                if(isCanalFirst==false){
                    canalObj1 = Instantiate(CanalPrefab1, transform.position, transform.rotation);
                    isCanalFirst = true;
                    NetworkServer.Spawn(canalObj1);  // 커널 스폰1
                }
                else{
                    canalObj2 = Instantiate(CanalPrefab2, transform.position, transform.rotation);
                    NetworkServer.Spawn(canalObj2);  // 커널 스폰2
                }
                
            }
            StartCoroutine("CreepDelayCoroutine");
        }
    }


    private IEnumerator ReloadBulletTimerCoroutine() {
        yield return new WaitForSeconds(1.2f);  //대기시간
        ReloadBullet = true;
        yield return null;
    }

    private IEnumerator CreepDelayCoroutine() {
        yield return new WaitForSeconds(1.5f);  //대기시간
        isSafeSkill = false;
        yield return null;
    }

    public int GetShotFlag() {
        return shotFlag;
    }

    private void AnimationSetting() {
        /// 플레이어가 이동 중 CCTV 화면 진입 시 위쪽 바라보게 설정
        if (hasAuthority && playerType == EPlayerType.Researcher && CCTV.isCCTV) {
            anim.SetBool("isInfection", false);
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", 1);
            shotFlag = 3;
        }
        else if (hasAuthority && playerType == EPlayerType.Infection && CCTV.isCCTV) {
            anim.SetBool("isInfection", true);
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", 1);
            shotFlag = 3;
        }
    }

    public void PlayerFaceChange(int flag) {
        if (randomSkill == 2) {

            // 감염체 -> 시민 변장 애니메이션 변경
            if (flag == 1) {
                anim.SetBool("isInfection", false);
                //FindFaceChange(hitFlag);
            }
            // 감염체 -> 시민 변장 애니메이션 변경 해제
            else {
                anim.SetBool("isInfection", true);
                //FindFaceChange(hitFlag);
                isSafeSkill = false;
            }
        }
    }
    /*
    //내가 지금 어디보고있는지, 보는 방향으로 캐릭터 모습 전환
    public void FindFaceChange(int hitFlag) {
        anim.SetBool("isChange", true);
        if (hitFlag == 1) {
            anim.SetInteger("hAxisRaw", 1);
        }
        else if (hitFlag == 2) {
            anim.SetInteger("hAxisRaw", 0);
        }
        else if (hitFlag == 3) {
            anim.SetInteger("vAxisRaw", 1);
        }
        else if (hitFlag == 4) {
            anim.SetInteger("vAxisRaw", 0);
        }
    }
    */

    private void MessageChatUpdate() {
        // 채팅내역 맨 아래 위치로 고정
        try {
            MessageChatController.Instance.scrollRect.normalizedPosition = new Vector2(0, 0);
        } catch {
            return;
        }
    }

    public void OnClickMessageButton() {
        if (MessageBoxSetting.activeMessageChat) {
            isChatting = true;
            InGameMenu.chatting = true;
            anim.SetInteger("hAxisRaw", 0);
            anim.SetInteger("vAxisRaw", 0);
        }
        else {
            isChatting = false;
            InGameMenu.chatting = false;
            return;
        }
    }

    public void Attack_Enabled() // 감염체 공격모션 1회 후 사라지게 하기
    {
        Infection_Attack_Right.SetActive(false);
        Infection_Attack_Left.SetActive(false);
        Infection_Attack_Up.SetActive(false);
        Infection_Attack_Down.SetActive(false);
    }
    /*
    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.5f);
        Infection_Attack_Direction.SetActive(false);
        check = true;
    }
    */
    public void InfectionAttackFunction(int flag) // 감염체 공격모션 함수
    {
        if (playerType == EPlayerType.Infection)  // 업데이트로 계속 불러줌, fire()가 눌러줄 때 cmd로 보내줌
        {
            //isTempInfectionAttack = false;
            CmdInfectionAttackFunction(flag);
        }

        //Invoke("Attack_Enabled", 0.5f); // 공격모션 사라지게 하는 함수
        //==================================== Invoke 대신 사용할려고 할 때 사용
        //check = false;
        //StartCoroutine(WaitForIt()); 
        //==================================
    }
    [Command(requiresAuthority = false)]
    public void CmdInfectionAttackFunction(int flag) // 감염체 공격모션 함수
    {
        if (isServer) {
            RpcInfectionAttackFunction(flag);
        }
    }
    [ClientRpc]
    public void RpcInfectionAttackFunction(int flag) // 감염체 공격모션 함수
    {
        PlayerFaceChange(2);
        if (flag == 1) {
            Infection_Attack_Right.SetActive(true); // 공격하면 공격모션 나타나게 해주는 함수
        }
        else if (flag == 2) {
            Infection_Attack_Right.SetActive(true); // 공격하면 공격모션 나타나게 해주는 함수
        }
        else if (flag == 3) {
            Infection_Attack_Up.SetActive(true); // 공격하면 공격모션 나타나게 해주는 함수
        }
        else if (flag == 4) {
            Infection_Attack_Down.SetActive(true); // 공격하면 공격모션 나타나게 해주는 함수
        }
        //isTempInfectionAttack = false;

        Invoke("Attack_Enabled", 0.5f); // 공격모션 사라지게 하는 함수
        //==================================== Invoke 대신 사용할려고 할 때 사용
        //check = false;
        //StartCoroutine(WaitForIt()); 
        //==================================
    }
}