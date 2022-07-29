using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 4); //think라는 함수를 5초 뒤에 호출
        //invoke() : 주어진 시간이 지난 뒤, 지정된 함수를 실행하는 함수
    }

    void FixedUpdate()
    {
        //기본 움직임
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //지형 체크(낭떠러지로 떨어지지 않게)
        Vector2 frontVec = new Vector2(rigid.position.x +nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null) //빔에 맞은게 없다면 = 앞에 아무것도 없다면
        {
            Turn();
        }
    }

    void Think()
    {
        //앞으로 갈지 멈출지 뒤로 갈지
        nextMove = Random.Range(-1, 2);

        //스프라이트 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);

        //방향 바꾸기
        if(nextMove != 0)
        spriteRenderer.flipX = nextMove == 1;

        //재귀함수
        float nextThinkTime = Random.Range(2f, 4f); //생각하는 시간 2초에서 5초 사이로 랜덤
        Invoke("Think", nextThinkTime); //재귀함수 : 자신을 스스로 호출하는 함수. 딜레이 없이 재귀함수 사용하는 것은 위험.
    }

    void Turn()
    {
        nextMove = nextMove * -1; //방향 바꾸기
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); //현재 작동 중인 모든 invoke함수를 멈추는 함수
        Invoke("Think", 2);
    }

    public void OnDamaged() //외부 함수 가져오는거니까 public
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //알파값(투명도)는 네번째

        spriteRenderer.flipY = true;

        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //몬스터 파괴
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

//플레이어가 점프한게 아니라 가까이만 가도 자꾸 몬스터가 죽음 ㅠ