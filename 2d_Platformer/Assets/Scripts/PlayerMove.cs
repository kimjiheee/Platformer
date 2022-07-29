using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager; //�÷��̾� ��ũ��Ʈ���� ���ӸŴ��� ���� ���� ���� ������ ����
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
        //������Ʈ �������� ���� �ʱ�ȭ��Ŵ
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string action)
    {
        //ȿ���� 
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
        //����
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))      //�ִϸ��̼��� �����ϰ� �ִ� ���°� �ƴ� ��(�̴����� ���� ���� �߰��� ����)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        //Ű ���� �� stop speed
        if(Input.GetButtonUp("Horizontal"))     //Ű�� ������
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);     //0.5 ������ �ӷ��� ���δ�
                                                      //normalized : ���� ũ�⸦ 1�� ���� ���� (��������). ���� ���� �� ���� ��..
                                        //rigid.velocity.normalized.x�� ���� ������ �ְ� 0.5f�� ũ�⸸ ��������
                                        //rigid.velocity.x�� ����, ũ�� ��� ������ �ִ�

        //���� ��ȯ
        if (Input.GetButton("Horizontal")) //GetButtonDown�� Ű �Է��� ��ġ�� �������� ���� �߻�. GetButton�� ��ư ���� �� �׻� üũ
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //walk �ִϸ��̼�
        if (Mathf.Abs(rigid.velocity.x) < 0.4) //����ٸ�
                //Abs�� ���밪
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //������ ����
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h*2, ForceMode2D.Impulse);

        //�ִ� �ּ� �ӵ� ����
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //�÷��� ����(����ĳ��Ʈ)
        if(rigid.velocity.y <0)     //y���� �ӵ��� �������� ���� ����
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));    //DrawRay(): �����ͻ󿡼��� ray�� �׷��ִ� �Լ�.(���� ���ӿ��� �� ����)
                //Drawray�� ��������, ����, �� ������ �ʿ�. -> ���� ��ġ����, �Ʒ� ��������, ������� ���̸� ���

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));    //�ʱ�ȭ
            //RaycastHit : Ray�� ���� ������Ʈ. ��������̶� Physics ��            //Layermask : ���� ȿ���� �����ϴ� ������, GetMask : ���̾� �̸��� �ش��ϴ� �������� �����ϴ� �Լ�
                                                                                   //Layermask.Getmask("�÷���") : �÷��� ���̾ ��� ���̸� üũ�� ��.

            if (rayHit.collider != null) //������ NULL�� �ƴ϶�� => �� ���� ����(�÷�����) �¾Ҵٸ� 
            {
                if (rayHit.distance < 0.5f)     //���� �÷����� ��Ҵٸ� (�÷��̾� ĸ�� ũ�Ⱑ 1�ε� ���̰� �߰����� �����ϱ� ������ 0.5�� �� ���鿡 ����) 
                                                //�� ĳ���Ͱ� �÷����� �������ִٸ� 
                    anim.SetBool("isJumping", false);   //���� �ִϸ��̼��� �ƴϵ���!
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Enemy")
        {
            //attack                  //transform:���� �÷��̾� ��ġ
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)  //���ͺ��� ���� ���� + �Ʒ��� ������
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
            //����
            bool isBronze = collision.gameObject.name.Contains("Bronze"); //Contains() : ��� ���ڿ��� �񱳹��� ������ true
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if(isGold)
                gameManager.stagePoint += 300;

            //������ �������
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //���� �ܰ�
            gameManager.NextStage();

            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        //����
        gameManager.stagePoint += 100;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos) // ���� �浹�� ���� ȿ�� �Լ�
                         //targetPosition
    {
        //��� ����
        gameManager.HealthDown();

        gameObject.layer = 11; //gameObject = �ڱ��ڽ�. PlayerDamaged ���̾�(11��° ���̾�)�� ����

        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //ĳ���������ϰ� ����

        //ƨ�ܳ���
                   //�÷��̾��� x��     //�ǰݴ��� ���� x��
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //����� 1(������) �ƴϸ� -1(����)
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged"); //�ǰ� �ִϸ��̼� 
        Invoke("OffDamaged", 1.5f); //���� �ð� 
    }

    void OffDamaged() //���� ���� ����
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //���İ�(����)�� �׹�°

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
