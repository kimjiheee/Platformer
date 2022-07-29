using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager; //플레이어 스크립트에서 게임매니저 변수 만들어서 점수 변수에 접근
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    void Awake()
    {
        //컴포넌트 가져오기 위해 초기화시킴
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string action)
    {
        //효과음 
        switch(action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    void Update()
    {
        //점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))      //애니메이션이 점프하고 있는 상태가 아닐 때(이단점프 막기 위해 추가한 조건)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        //키 뗐을 때 stop speed
        if(Input.GetButtonUp("Horizontal"))     //키를 뗐으면
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);     //0.5 정도로 속력을 줄인다
                                                      //normalized : 벡터 크기를 1로 만든 상태 (단위벡터). 방향 구할 때 쓰는 것..
                                        //rigid.velocity.normalized.x은 방향 가지고 있고 0.5f로 크기만 결정해줌
                                        //rigid.velocity.x는 방향, 크기 모두 가지고 있다

        //방향 전환
        if (Input.GetButton("Horizontal")) //GetButtonDown은 키 입력이 겹치는 구간에서 문제 발생. GetButton은 버튼 누를 때 항상 체크
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //walk 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.4) //멈췄다면
                //Abs는 절대값
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //움직임 구현
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h*2, ForceMode2D.Impulse);

        //최대 최소 속도 제한
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //플랫폼 착지(레이캐스트)
        if(rigid.velocity.y <0)     //y축의 속도가 내려가는 중일 때만
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));    //DrawRay(): 에디터상에서만 ray를 그려주는 함수.(실제 게임에선 안 보임)
                //Drawray는 시작지점, 방향, 색 세개가 필요. -> 지금 위치에서, 아래 방향으로, 녹색으로 레이를 쏜다

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));    //초기화
            //RaycastHit : Ray에 닿은 오브젝트. 물리기반이라 Physics 씀            //Layermask : 물리 효과를 구분하는 정수값, GetMask : 레이어 이름에 해당하는 정수값을 리턴하는 함수
                                                                                   //Layermask.Getmask("플랫폼") : 플랫폼 레이어에 닿는 레이만 체크할 것.

            if (rayHit.collider != null) //닿은게 NULL이 아니라면 => 즉 빔에 뭔가(플랫폼이) 맞았다면 
            {
                if (rayHit.distance < 0.5f)     //빔이 플랫폼에 닿았다면 (플레이어 캡슐 크기가 1인데 레이가 중간부터 시작하기 때문에 0.5면 딱 지면에 닿음) 
                                                //즉 캐릭터가 플랫폼에 착지해있다면 
                    anim.SetBool("isJumping", false);   //점프 애니메이션이 아니도록!
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Enemy")
        {
            //attack                  //transform:현재 플레이어 위치
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)  //몬스터보다 위에 있음 + 아래로 낙하중
                OnAttack(collision.transform);
            else //damaged
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //점수
            bool isBronze = collision.gameObject.name.Contains("Bronze"); //Contains() : 대상 문자열에 비교문이 있으면 true
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if(isGold)
                gameManager.stagePoint += 300;

            //아이템 사라지기
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //다음 단계
            gameManager.NextStage();

            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        //점수
        gameManager.stagePoint += 100;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos) // 적과 충돌시 무적 효과 함수
                         //targetPosition
    {
        //목숨 깎음
        gameManager.HealthDown();

        gameObject.layer = 11; //gameObject = 자기자신. PlayerDamaged 레이어(11번째 레이어)로 변경

        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //캐릭터투명하게 만듦

        //튕겨나감
                   //플레이어의 x축     //피격당한 적의 x축
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //양수면 1(오른쪽) 아니면 -1(왼쪽)
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged"); //피격 애니메이션 
        Invoke("OffDamaged", 1.5f); //무적 시간 
    }

    void OffDamaged() //무적 상태 해제
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //알파값(투명도)는 네번째

        spriteRenderer.flipY = true;

        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Sound
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
