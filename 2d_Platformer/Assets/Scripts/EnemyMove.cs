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

        Invoke("Think", 4); //think��� �Լ��� 5�� �ڿ� ȣ��
        //invoke() : �־��� �ð��� ���� ��, ������ �Լ��� �����ϴ� �Լ�
    }

    void FixedUpdate()
    {
        //�⺻ ������
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //���� üũ(���������� �������� �ʰ�)
        Vector2 frontVec = new Vector2(rigid.position.x +nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null) //���� ������ ���ٸ� = �տ� �ƹ��͵� ���ٸ�
        {
            Turn();
        }
    }

    void Think()
    {
        //������ ���� ������ �ڷ� ����
        nextMove = Random.Range(-1, 2);

        //��������Ʈ �ִϸ��̼�
        anim.SetInteger("WalkSpeed", nextMove);

        //���� �ٲٱ�
        if(nextMove != 0)
        spriteRenderer.flipX = nextMove == 1;

        //����Լ�
        float nextThinkTime = Random.Range(2f, 4f); //�����ϴ� �ð� 2�ʿ��� 5�� ���̷� ����
        Invoke("Think", nextThinkTime); //����Լ� : �ڽ��� ������ ȣ���ϴ� �Լ�. ������ ���� ����Լ� ����ϴ� ���� ����.
    }

    void Turn()
    {
        nextMove = nextMove * -1; //���� �ٲٱ�
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); //���� �۵� ���� ��� invoke�Լ��� ���ߴ� �Լ�
        Invoke("Think", 2);
    }

    public void OnDamaged() //�ܺ� �Լ� �������°Ŵϱ� public
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //���İ�(����)�� �׹�°

        spriteRenderer.flipY = true;

        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //���� �ı�
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

//�÷��̾ �����Ѱ� �ƴ϶� �����̸� ���� �ڲ� ���Ͱ� ���� ��